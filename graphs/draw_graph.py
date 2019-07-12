import pandas as pd
import numpy as np
import networkx as nx
import math
import sys,os

f_0 = open('attack_1_'+sys.argv[1][0]+'.txt','r')
f_1 = open('attack_1_'+sys.argv[1][2]+'.txt','r')

line_0 = f_0.readline()
line_0 = f_0.readline()
line_1 = f_1.readline()
line_1 = f_1.readline()

pathset_0 = []
pathset_1 = []
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
            if(len(path_0) < len(path_1)):
                pathset_1.append(path_1)
            else:
                pathset_1.append(path_0)
        path_0 = []
        path_1 = []

f_0.close()
f_1.close()

df_0 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_0])
df_1 = pd.concat([pd.DataFrame(data = {'src':path[:-1], 'dst':path[1:]}) for path in pathset_1])
df_0['weight'] = [1] * (df_0.shape)[0]
df_1['weight'] = [1] * (df_1.shape)[0]

df_0 = df_0.groupby(['src','dst']).sum()
df_0 = df_0.reset_index()
#df_0['weight'] = df_0['weight'].apply(lambda x: math.log(x) + 1)
df_1 = df_1.groupby(['src','dst']).sum()
df_1 = df_1.reset_index()
#df_1['weight'] = df_1['weight'].apply(lambda x: math.log(x) + 1)

#df_0['color'] = ['b'] * (df_0.shape)[0]
#df_1['color'] = ['r'] * (df_1.shape)[0]

#common = df_0[(df_0.src.isin(df_1.src))&(df_0.dst.isin(df_1.dst))]
#df_00 = df_0[(~df_0.src.isin(df_1.src))|(~df_0.dst.isin(df_1.dst))]
#df_11 = df_1[(~df_1.src.isin(df_0.src))|(~df_1.dst.isin(df_0.dst))]
#common['color'] = common['color'].apply(lambda x: 'k')
#df = pd.concat([df_00,df_11,common])

#L: 9009(rank:110), 9121(rank:130)
#M: 6130(rank:1246),8001(rank:968)
#S: 9008(rank:4049),21232(rank:3514)

G = nx.from_pandas_edgelist(df, 'src', 'dst', ['weight','color'])
#print(G.nodes)
G.node[9121]['c'] = '1'
G.node[21232]['c'] = '1'
nx.write_gexf(G,sys.argv[1]+'.gexf')
