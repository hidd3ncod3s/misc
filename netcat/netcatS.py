#!/usr/bin/env python

import sys
import os
import socket
import httplib

def netcatS(hostname, port, content):
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	s.connect((hostname, port))
	sslSocket = socket.ssl(s)
	print "\nRequest: "
	print content
	sslSocket.write(content)
	print "\nResponse: "
	print sslSocket.read()
	s.close()

def usage():
    print '''
Usage: %s hostname port filename
    '''.strip() % sys.argv[0]

if len(sys.argv) < 4 or not os.path.isfile(sys.argv[3]):
    #print len(sys.argv)
    usage()
    sys.exit()

netcatS(sys.argv[1], int(sys.argv[2]), file(sys.argv[3], 'rb').read())