import xlwt
from xlwt import Workbook
import statistics
import pandas as pd
import numpy as np
import networkx as nx
import math
import sys,os

dest_asns_dict = {
'L':['28598','12714','4788','5617','7713','3255','8732','29049','1257','8708'],
'M':['1103','57','24538','9905','28338','20141','28624','262462','15744','61889'],
'S':['13230','2048','35913','38390','11003','2635','17803','17589','44208','37981']}

wb = Workbook()
sheet = wb.add_sheet(sys.argv[1]+'_'+sys.argv[2])
ftxt = open('statistics/'+sys.argv[1]+'_'+sys.argv[2]+'.txt', 'w')

for index0 in range(10):
    index_list = list(range(10))[(1-index0%2):10:2]
    for index1 in index_list:

        node_0 = int(dest_asns_dict[sys.argv[1]][index0])
        node_1 = int(dest_asns_dict[sys.argv[2]][index1])

        f_0 = open('normal/result_%d_%s_0.txt' % (index0, sys.argv[1]),'r')
        f_1 = open('attack/result_%d_%s_%d_%s_1.txt' % (index0, sys.argv[1], index1, sys.argv[2]),'r')

        pathset_11 = []
        pathset_10 = []
        pathset_00 = []
        pathset_01 = []
        pathset_1 = []
        pathset_0 = []

        path_0 = []
        path_1 = []

        line_0 = f_0.readline()
        while (line_0 != ""):
            if('0' < line_0[0] <= '9'):
                path_0.append(int(line_0[:-1]))
                line_0 = f_0.readline()
            else:
                if(len(path_0) > 1):
                    #path_set_0[str(path_0[0])] = path_0
                    pathset_0.append(path_0)
                    path_0 = []
                while(not ('0' < line_0[0] <= '9')):
                    line_0 = f_0.readline()
                    if(line_0 == ''):
                        break

        line_1 = f_1.readline()
        while (line_1 != ""):
            if('0' < line_1[0] <= '9'):
                path_1.append(int(line_1[:-1]))
                line_1 = f_1.readline()
            else:
                if(len(path_1) > 1):
                    #path_set_1[str(path_1[0])] = path_1
                    pathset_1.append(path_1)
                    path_1 = []
                while(not ('0' < line_1[0] <= '9')):
                    line_1 = f_1.readline()
                    if(line_1 == ''):
                        break

        f_0.close()
        f_1.close()

        for path in pathset_0:
            if node_0 in path:
                pathset_00.append(path[0:path.index(node_0)+1])
            if node_1 in path:
                pathset_01.append(path[0:path.index(node_1)+1])

        for path in pathset_1:
            if node_0 in path:
                pathset_10.append(path[0:path.index(node_0)+1])
            if node_1 in path:
                pathset_11.append(path[0:path.index(node_1)+1])

#------------------------------------------------------------------------------------------------------------------
        #print('#g10')
        if(len(pathset_10) > 0):
            df_10 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_10])
            df_10 = df_10.drop_duplicates()
            g10 = nx.from_pandas_edgelist(df_10, 'src', 'dst')
        else:
            g10 = nx.Graph()

        #print('#g11')
        if(len(pathset_11) > 0):
            df_11 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_11])
            df_11 = df_11.drop_duplicates()
            g11 = nx.from_pandas_edgelist(df_11, 'src', 'dst')
        else:
            g11 = nx.Graph()

        #print('#g01')
        if(len(pathset_01) > 0):
            df_01 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_01])
            df_01 = df_01.drop_duplicates()
            g01 = nx.from_pandas_edgelist(df_01, 'src', 'dst')
        else:
    	    g01 = nx.Graph()

        #print('#g00')
        if(len(pathset_00) > 0):
            df_00 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_00])
            df_00 = df_00.drop_duplicates()
            g00 = nx.from_pandas_edgelist(df_00, 'src', 'dst')
        else:
            g00 = nx.Graph()

        g0 = nx.compose(g00,g01)
        g0.add_edge(node_0,10)
        g1 = nx.compose(g10,g11)
        g1.add_edge(node_0,10)
        g1.add_edge(node_1,10)
#-------------------------------------------------------------------------------------

        degrees = [val for (node, val) in g0.degree()]
        avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        depth_candidate = [len(path) for path in pathset_0]
        depth = max(depth_candidate)
        print(index0*5+int(index1/2))
        sheet.write(index0*5+int(index1/2), 0, avg_degrees)
        sheet.write(index0*5+int(index1/2), 1, len(g0.nodes))
        sheet.write(index0*5+int(index1/2), 2, depth)
        print('|', avg_degrees,'|', len(g0.nodes),'|', depth, end='')
        ftxt.write(str(avg_degrees)+' '+str(len(g0.nodes))+' '+str(depth)+' ')

        degrees = [val for (node, val) in g1.degree()]
        avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        depth_candidate = [len(path) for path in pathset_10 + pathset_11]
        depth = max(depth_candidate)
        sheet.write(index0*5+int(index1/2), 3, avg_degrees)
        sheet.write(index0*5+int(index1/2), 4, len(g1.nodes))
        sheet.write(index0*5+int(index1/2), 5, depth)
        print('|', avg_degrees, '|', len(g1.nodes), '|', depth, end='')
        ftxt.write(str(avg_degrees)+' '+str(len(g1.nodes))+' '+str(depth)+' ')

        degrees = [val for (node, val) in g00.degree()]
        avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        depth_candidate = [len(path) for path in pathset_00]
        depth = max(depth_candidate)
        sheet.write(index0*5+int(index1/2), 6, avg_degrees)
        sheet.write(index0*5+int(index1/2), 7, len(g00.nodes))
        sheet.write(index0*5+int(index1/2), 8, depth)
        print('|', avg_degrees, '|', len(g00.nodes), '|', depth, end='')
        ftxt.write(str(avg_degrees)+' '+str(len(g00.nodes))+' '+str(depth)+' ')

        degrees = [val for (node, val) in g10.degree()]
        avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        depth_candidate = [len(path) for path in pathset_10]
        depth = max(depth_candidate)
        sheet.write(index0*5+int(index1/2), 9, avg_degrees)
        sheet.write(index0*5+int(index1/2), 10, len(g10.nodes))
        sheet.write(index0*5+int(index1/2), 11, depth)
        print('|', avg_degrees, '|', len(g10.nodes), '|', depth, end='')
        ftxt.write(str(avg_degrees)+' '+str(len(g10.nodes))+' '+str(depth)+' ')

        degrees = [val for (node, val) in g01.degree()]
        if(len(degrees) > 0):
            avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        else:
            avg_degrees = 0
        depth_candidate = [len(path) for path in pathset_01]
        if(len(depth_candidate) > 0):
            depth = max(depth_candidate)
        else:
            depth = 0
        sheet.write(index0*5+int(index1/2), 12, avg_degrees)
        sheet.write(index0*5+int(index1/2), 13, len(g01.nodes))
        sheet.write(index0*5+int(index1/2), 14, depth)
        print('|', avg_degrees, '|', len(g01.nodes),'|', depth, end='')
        ftxt.write(str(avg_degrees)+' '+str(len(g01.nodes))+' '+str(depth)+' ')

        degrees = [val for (node, val) in g11.degree()]
        if(len(degrees) > 0):
            avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        else:
            avg_degrees = 0
        depth_candidate = [len(path) for path in pathset_11]
        if(len(depth_candidate) > 0):
            depth = max(depth_candidate)
        else:
            depth = 0
        sheet.write(index0*5+int(index1/2), 15, avg_degrees)
        sheet.write(index0*5+int(index1/2), 16, len(g11.nodes))
        sheet.write(index0*5+int(index1/2), 17, depth)
        print('|', avg_degrees, '|', len(g11.nodes),'|', depth, '|')
        ftxt.write(str(avg_degrees)+' '+str(len(g11.nodes))+' '+str(depth)+'\n')

ftxt.close()
wb.save('statistics/'+sys.argv[1]+'_'+sys.argv[2]+'.xls')
