#!/usr/bin/env python
# Dridex_config_decoder2.py
#                  _                          _
#  _ __ ___   __ _| |_      ____ _ _ __ ___  | |_   _
# | '_ ` _ \ / _` | \ \ /\ / / _` | '__/ _ \ | | | | |
# | | | | | | (_| | |\ V  V / (_| | | |  __/_| | |_| |
# |_| |_| |_|\__,_|_| \_/\_/ \__,_|_|  \___(_)_|\__,_|

import sys
import tempfile
import os
import pefile
from aplib import decompress
import socket


LEN_XOR_KEY = 4  # Dridex has a XOR key of 4 bytes.

def getFileasBA(pe_file):
	return bytearray(open(pe_file, "rb").read())

def extract_sdata(pe_file):
    pe = pefile.PE(pe_file)
    for section in pe.sections:
        if '.sdata' in section.Name:
            return bytearray(section.get_data(section.VirtualAddress, section.SizeOfRawData))
    return None


def xor_decode(buffer, xor_key='', len_key=LEN_XOR_KEY):
    return bytearray(buffer[i] ^ int(xor_key[i % len_key], 16) for i in range(len(buffer)))


def decompress_that_shit(buffer, output='config.txt'):
    # AWFUL hack because of aplib bug with `str` and offset... forced to use `file` instead.
    temp_file = tempfile.TemporaryFile(mode='w+b')
    # Skip 12 first bytes (4 bytes for XOR key, 4 for compressed size and 4 for uncompressed size)
    temp_file.write(buffer[12:])
    temp_file.seek(0)
    cleartext = decompress(temp_file).do()
    temp_file.close()
    print('[I] Decoded configuration:\n\t %s' % str(cleartext))
    with open(output, 'wb') as f:
        print("[I] Saving decoded configuration into '%s'" % output)
        f.write(str(cleartext))

def usage(file):
	print('Usage: %s <dridex_dump> [<output>]' % file)
	sys.exit(-1)

import struct
if __name__ == '__main__':
    if len(sys.argv) < 2 or not os.path.isfile(sys.argv[1]):
        usage(sys.argv[0])

    print("[I] Extracting '.sdata' section.")
    resource = extract_sdata(sys.argv[1])
    if resource:
        xor_key = [hex(x) for x in resource[:LEN_XOR_KEY]]
        print("[I] XOR key: '%s'" % str(xor_key))
        resource = xor_decode(resource, xor_key=xor_key)
        if len(sys.argv) > 2:
            decompress_that_shit(resource, output=sys.argv[2])
        else:
            decompress_that_shit(resource)
        sys.exit(0)
    else:
        # Try method2.
        print("[I] Trying second method.")
        
        ba = getFileasBA(sys.argv[1])
		
        magicnearconfiguration = bytearray((int(0x56), int(0x57), int(0x53), int(0x55), int(0x81), int(0xEC), int(0x80), int(0x00)))
        magic_start_loc= ba.find(magicnearconfiguration)
        if magic_start_loc != -1:
            configuration= ba[magic_start_loc-92:magic_start_loc]
            #print(''.join('%02x '% x for x in configuration))
			print(''.join('%02x '% x for x in configuration[:2]))
            botid= struct.unpack('h', configuration[:2])[0]
            numberofservers= struct.unpack('b', configuration[2:3])[0]
            print "Bot_ID: %d" % botid
            print "Number of Servers: %d" % numberofservers
            ipconfigurations= configuration[4:]
            while len(ipconfigurations) >= 6:
                #print "length= " + str(len(ipconfigurations))
                ipaddr_i= struct.unpack('>I', ipconfigurations[:4])[0]
                port_i= struct.unpack('H', ipconfigurations[4:6])[0]
                print "Server= %s:%d" % (socket.inet_ntoa(struct.pack('!L', ipaddr_i)) , port_i)
                ipconfigurations= ipconfigurations[6:]