import socket
import sys
import os
import re
import string

def netcat(hostname, port, content):
	print content
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	s.connect((hostname, port))
	s.sendall(content)
	#s.shutdown(socket.SHUT_WR)
	while 1:
		data = s.recv(1024)
		if data == "":
			break
		print "Received:", repr(data)
	print "Connection closed."
	s.close()

def usage():
    print '''
Usage: %s hostname port filename
    '''.strip() % sys.argv[0]

if len(sys.argv) < 4 or not os.path.isfile(sys.argv[3]):
    #print len(sys.argv)
    usage()
    sys.exit()


netcat(sys.argv[1], int(sys.argv[2]), file(sys.argv[3], 'rb').read())