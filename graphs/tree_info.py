import statistics
import pandas as pd
import numpy as np
import networkx as nx
import math
import sys,os

f_0 = open('attack_0_'+sys.argv[1][0]+'.txt','r')
f_1 = open('attack_1_'+sys.argv[1][2]+'.txt','r')

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
avg_degrees = int(statistics.mean(degrees) * 1000)/1000
depth_candidate = [len(path) for path in pathset_0]
depth = max(depth_candidate)
print('|', avg_degrees,'|', len(g0.nodes),'|', depth, end='')

degrees = [val for (node, val) in g1.degree()]
avg_degrees = int(statistics.mean(degrees) * 1000)/1000
depth_candidate = [len(path) for path in pathset_10 + pathset_11]
depth = max(depth_candidate)
print('|', avg_degrees, '|', len(g1.nodes), '|', depth, end='')

degrees = [val for (node, val) in g10.degree()]
avg_degrees = int(statistics.mean(degrees) * 1000)/1000
depth_candidate = [len(path) for path in pathset_10]
depth = max(depth_candidate)
print('|', avg_degrees, '|', len(g10.nodes), '|', depth, end='')

degrees = [val for (node, val) in g11.degree()]
avg_degrees = int(statistics.mean(degrees) * 1000)/1000
depth_candidate = [len(path) for path in pathset_11]
depth = max(depth_candidate)
print('|', avg_degrees, '|', len(g11.nodes),'|', depth, '|')

#print(nx.is_tree(g10))
#print(nx.is_tree(g11))
