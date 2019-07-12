import sys, os

fin = open('result_1000.txt','r')

i = 10
fout = open('result_1000_'+str(i), 'w')

line = fin.readline()
while(line != ''):
    if(line[0:4] != 'Wait'):
        fout.write(line)
    else:
        fout.close()
        i += 1
        fout = open('result_1000_'+str(i), 'w')
        line = fin.readline()
    line = fin.readline()

fin.close()
fout.close()
