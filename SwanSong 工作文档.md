# SwanSong 工作文档

[TOC]

## 通用类文档

### Unit:单位基类



![image-20221022163550602](C:\Users\14621\AppData\Roaming\Typora\typora-user-images\image-20221022163550602.png)

#### 主要字段

1. UnitData

   角色的数值数据，如血量等

2. Scheduler

   卡牌调度器，可执行抽弃牌等功能

#### 主要功能

1. 添加buff

   调用AddBuff，传入要赋给单位的效果

2. 设置位置

   直接给Position进行赋值为想要传送到的坐标

3. 施加伤害

   调用Hurt传入伤害点数

4. 治愈

   调用Cure传入治愈点数

### Map:战斗地图

![image-20221022212302347](C:\Users\14621\AppData\Roaming\Typora\typora-user-images\image-20221022212302347.png)

#### 主要功能

通过索引器传入[x,y]获取指定坐标的Tile

### Tile:图块

![image-20221022212507766](C:\Users\14621\AppData\Roaming\Typora\typora-user-images\image-20221022212507766.png)

#### 主要字段

1. Units

   位于该图块上的单位

## 路线图生成工作内容

![image-20221022214840851](C:\Users\14621\AppData\Roaming\Typora\typora-user-images\image-20221022214840851.png)

​	TreeMap的结构是一棵以TreeMapNode为节点的树，节点内部存有PlaceType枚举来说明该节点所属的功能类型。	

​	这个工作需要实现TreeMapFactory的CreateTreeMap方法，这个方法需要根据外界传入的地图参数（地图参数内容由你们决定）来生成TreeMap。

## 卡牌脚本工作内容

![image-20221022220050491](C:\Users\14621\AppData\Roaming\Typora\typora-user-images\image-20221022220050491.png)

​	对于每一张卡牌你需要继承Card基类并重写GetAffectTarget、GetAvaliableTarget、Release三个方法

 1. GetAvaliableTarget

    ​	这个方法会传入一个Unit作为释放该卡牌的单位，你需要生成并返回一个TargetData，里面包含了该卡牌的施法范围与可选目标的二维坐标数组

 2. GetAffectTarget

    ​	这个方法会传入一个Unit作为释放该卡牌的单位与一个二维坐标作为施法目标的坐标，你需要生成一个所有将受到该卡牌影响的单位或图块的坐标集合

 3. Release

    ​	这个方法会传入一个Unit作为释放该卡牌的单位与一个二维坐标作为施法目标的坐标，你需要在这个方法中实现卡牌释放的具体逻辑，比如对一个单位造成伤害等。

## AI工作内容

​	继承Unit基类并重写Decide方法，实现单位在每回合的行为逻辑，如移动到某个地点、对某个单位释放卡牌等
