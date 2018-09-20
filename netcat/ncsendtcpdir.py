import socket
import sys
import os
import re
import string

def netcat(hostname, port, content):
	#print content
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	s.connect((hostname, port))
	s.setblocking(0)
	s.sendall(content)
	#s.shutdown(socket.SHUT_WR)
	while 0:
		#s.settimeout(1)
		try:
			data = s.recv(1024)
			if data == "":
				break
		except:
			break
		#print "Received:", repr(data)
	print "Connection closed."
	s.close()

def usage():
    print '''
Usage: %s hostname port filename/dirname
    '''.strip() % sys.argv[0]

if len(sys.argv) < 4 :
    #print len(sys.argv)
    usage()
    sys.exit()

if os.path.isfile(sys.argv[3]):
	netcat(sys.argv[1], int(sys.argv[2]), file(sys.argv[3], 'rb').read())
	exit(0)

fileList = []
for root, subFolders, files in os.walk(sys.argv[3]):
    for file1 in files:
        fileList.append(os.path.join(root,file1))

for filename in fileList:
	print "File : %s"%filename;
	netcat(sys.argv[1], int(sys.argv[2]), file(filename, 'rb').read())