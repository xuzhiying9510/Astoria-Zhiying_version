import socket
import pickle
import sys, os

TCP_IP = '::1'
TCP_PORT = 11000

dest_asns_dict = {
        'L':['28598','12714','4788','5617','7713','3255','8732','29049','1257','8708'],
        'M':['1103','57','24538','9905','28338','20141','28624','262462','15744','61889'],
        'S':['13230','2048','35913','38390','11003','2635','17803','17589','44208','37981']}

with open("ASes.txt", "rb") as fp:
        asn_srcs = pickle.load(fp)
asn_srcs = list(map(str,asn_srcs))
for size in ['S','M','L']:
    for i in range(10):
        if dest_asns_dict[size][i] in asn_srcs:
            asn_srcs.remove(dest_asns_dict[size][i])
asn_srcs = asn_srcs[1:50000:10]

for dest_asn in dest_asns_dict[sys.argv[1]]:
    MESSAGE = ""
    count = 0

    for asn_src in asn_srcs:
        if asn_src == dest_asn:
            continue
        MESSAGE += " " + asn_src + " " + dest_asn
        count += 1
    MESSAGE += " <EOFc> "
    print "Sending message to BGPSim to get %d paths towards %s.." % (count, dest_asn)

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

