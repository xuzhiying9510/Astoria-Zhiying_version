import socket
import pickle
import sys, os

dest_asns_dict = {
        'L':['28598','12714','4788','5617','7713','3255','8732','29049','1257','8708'],
        'M':['1103','57','24538','9905','28338','20141','28624','262462','15744','61889'],
        'S':['13230','2048','35913','38390','11003','2635','17803','17589','44208','37981']}

for size in ['L','M','S']:
    for i in range(10):
        node = dest_asns_dict[size][i]
        fin = open('TestingApplication/bin/Release/Cyclops_caida_cons.txt','r')
        fout = open('Cyclops_caida_cons.txt','w')
        line = fin.readline()
        while(line != ''):
            line_set = line.split()
            if(not '10' in line_set):
                fout.write(line)
            line = fin.readline()
        fout.write(node+'\t10\tp2c\n')

        fin.close()
        fout.close()

        os.system('mono TestingApplication/bin/Debug/TestingApplication.exe -server11000 Cyclops_caida_cons.txt US-precomp367.txt exit_asns.txt > result_%d_%s_0.txt &' % (i, size))
        os.system('sleep 5s')
        os.system('python client.py ' + '10')
        os.system('killall mono')
