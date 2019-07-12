#L: 9009(rank:110), 9121(rank:130)
#M: 6130(rank:1246),8001(rank:968)
#S: 9008(rank:4049),21232(rank:3514)

import socket
import pickle

TCP_IP = '::1'
TCP_PORT = 11000

with open("ASes.txt", "rb") as fp:
        asn_srcs = pickle.load(fp)
asn_srcs = list(map(str,asn_srcs))

dest_asn = '21232'
MESSAGE = ""
count = 0

for asn_src in asn_srcs:
    if asn_src == dest_asn:
        continue
    MESSAGE += " " + asn_src + " " + dest_asn
    count += 1
MESSAGE += " <EOFc> "
print "Sending message to BGPSim to get %d paths towards %s.." % (count, dest_asn)
'''
s = socket.socket(socket.AF_INET6, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))
s.send(MESSAGE)

data = ""
result = dict()
buffer_size = 10000000
while True:
    d = s.recv(buffer_size)
    data += d
    if len(d) == 0:
        break
    if "<EOFs>" in d:
        break

s.close()
'''
