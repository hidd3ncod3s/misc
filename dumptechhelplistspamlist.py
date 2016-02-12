import re
from multiprocessing.pool import ThreadPool as Pool
import requests
import bs4
from urlparse import urljoin
import pprint

root_url = 'https://techhelplist.com'
index_url = root_url + '/spam-list/'

spamentrylist= []

def get_spam_page_urllist():
	response = requests.get(index_url)
	soup = bs4.BeautifulSoup(response.text, "html.parser")
	trs = soup.find_all('tr')
	
	table= soup.find('tbody')
	if table == None:
		return;
	
	trs = table.find_all('tr')
	for tr in trs:
		tds= tr.find_all('td');
		href= tds[0].a.get('href')
		date= tds[1].get_text().strip()
		spamentrylist.append({'url': urljoin(index_url, href), 'date': date, 'Hashes': []})
	
def get_spam_data(spam_data):
	response = requests.get(spam_data['url'])
	soup = bs4.BeautifulSoup(response.text, "html.parser")
	links = soup.find_all('a')
	
	for link in links:
		href= link.get('href')
		if href != None:
			if re.search('www.virustotal.com', href, re.IGNORECASE):
				href= href.split('/')[5]
				spam_data['Hashes'].append(href)
				
def dump_spaminfo():
	pool = Pool()
	get_spam_page_urllist()
	[pool.apply_async(get_spam_data, args=(x,), callback = lambda x: None) for x in spamentrylist]
	pool.close()
	pool.join()
	
	spamentrylist.sort(key=lambda x: x['date'], reverse=True)
	
	for entry in spamentrylist:
		if len(entry['Hashes']) == 0:
			continue
		
		print "URL   : " + entry['url']
		print "Date  : " + entry['date']
		print "Hashes:"
		
		for hash in entry['Hashes']:
			print "\t" + hash
		
		print "\n"
	return

if __name__ == '__main__':
    dump_spaminfo()