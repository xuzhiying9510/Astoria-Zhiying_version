import xlwt
from xlwt import Workbook
import statistics
import pandas as pd
import numpy as np
import networkx as nx
import math
import sys,os

wb = Workbook()
sheet = wb.add_sheet('victim_'+sys.argv[1][0]+'_attacker_'+sys.argv[1][2])

for index0 in range(10):
    index1_list = list( range(10))
    index1_list.remove(index0)
    index1_list = index1_list[0:9:2]
    for index1 in range(5):
        f_0 = open(sys.argv[1][0]+'/result_1000_'+str(index0),'r')
        f_1 = open(sys.argv[1][2]+'/result_1000_'+str(index1_list[index1]),'r')

        line_0 = f_0.readline()
        line_0 = f_0.readline()
        line_1 = f_1.readline()
        line_1 = f_1.readline()

        pathset_0 = []
        pathset_11 = []
        pathset_10 = []
        path_0 = []
        path_1 = []
        while (line_0 != "" and line_1 != ''):
            if('0' < line_0[0] <= '9'):
                path_0.append(int(line_0[:-1]))
                line_0 = f_0.readline()
            if('0' < line_1[0] <= '9'):
                path_1.append(int(line_1[:-1]))
                line_1 = f_1.readline()
            if(line_0[0] == '-' and line_1[0] == '-'):
                line_0 = f_0.readline()
                line_0 = f_0.readline()
                line_1 = f_1.readline()
                line_1 = f_1.readline()
                if(len(path_0) > 1 and len(path_1) > 1):
                    pathset_0.append(path_0)
                    if(len(path_0) <= len(path_1)):
                         pathset_10.append(path_0)
                    else:
                        pathset_11.append(path_1)
                path_0 = []
                path_1 = []
        f_0.close()
        f_1.close()


        df_0 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_0])
        df_0 = df_0.drop_duplicates()
        g0 = nx.from_pandas_edgelist(df_0, 'src', 'dst')

        if(len(pathset_10) > 0):
            df_10 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_10])
            df_10 = df_10.drop_duplicates()
            g10 = nx.from_pandas_edgelist(df_10, 'src', 'dst')
        else:
            g10 = nx.Graph()

        if(len(pathset_11) > 0):
            df_11 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_11])
            df_11 = df_11.drop_duplicates()
            g11 = nx.from_pandas_edgelist(df_11, 'src', 'dst')
        else:
            g11 = nx.Graph()

        df_1 = pd.concat([df_10,df_11])
        df_1 = df_1.drop_duplicates()
        g1 = nx.from_pandas_edgelist(df_1, 'src', 'dst')

        degrees = [val for (node, val) in g0.degree()]
        avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        depth_candidate = [len(path) for path in pathset_0]
        depth = max(depth_candidate)
        print(index0*5+index1)
        sheet.write(index0*5+index1, 0, 'Experiment '+ str(index0*5+index1))
        sheet.write(index0*5+index1, 1, avg_degrees)
        sheet.write(index0*5+index1, 2, len(g0.nodes))
        sheet.write(index0*5+index1, 3, depth)
        print('|', avg_degrees,'|', len(g0.nodes),'|', depth, end='')

        degrees = [val for (node, val) in g1.degree()]
        avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        depth_candidate = [len(path) for path in pathset_10 + pathset_11]
        depth = max(depth_candidate)
        sheet.write(index0*5+index1, 4, avg_degrees)
        sheet.write(index0*5+index1, 5, len(g1.nodes))
        sheet.write(index0*5+index1, 6, depth)
        print('|', avg_degrees, '|', len(g1.nodes), '|', depth, end='')

        degrees = [val for (node, val) in g10.degree()]
        avg_degrees = int(statistics.mean(degrees) * 10000)/10000
        depth_candidate = [len(path) for path in pathset_10]
        depth = max(depth_candidate)
        sheet.write(index0*5+index1, 7, avg_degrees)
        sheet.write(index0*5+index1, 8, len(g10.nodes))
        sheet.write(index0*5+index1, 9, depth)
        print('|', avg_degrees, '|', len(g10.nodes), '|', depth, end='')

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
        sheet.write(index0*5+index1, 10, avg_degrees)
        sheet.write(index0*5+index1, 11, len(g11.nodes))
        sheet.write(index0*5+index1, 12, depth)
        print('|', avg_degrees, '|', len(g11.nodes),'|', depth, '|')

wb.save('tree_info_'+sys.argv[1]+'.xls')
        #print(nx.is_tree(g10))
        #print(nx.is_tree(g11))
