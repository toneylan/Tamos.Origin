## Tamos.Origin
适用于.Net framework和.Net Core应用开发的基础设施框架，目标是帮助开发人员更专注于应用需求的开发，而不用过多关注基础功能的实现，如缓存、集中式配置、分布式计算、IOC、Rpc等，
以提升编码效率。

比较适合中小型项目的快速开发，也注重降低运行时性能消耗。目前此项目主要是内部团队使用，没有进行非常通用化的设计，希望能提供参考和交流。

#### Tamos.AbleOrigin
- 核心库，包含一些工具类函数。同时抽象了各基础功能，以解耦引用的一些第三方开源库，在Tamos.AbleOrigin.Booster中进行了具体实现。

#### Tamos.AbleOrigin.DataPersist
- 基于Entity Framework Core的数据访问库，目前使用的Mysql。
- 支持按不同时间维度分表，如三个月、一年。
- FastAccessor 实现Entity+DTO模式的快捷数据库操作，可简化掉增删改查等常规EF Core语句。

#### Grpc封装
- 调用层直接从IOC获取接口实例即可，几乎感知不到底层Grpc。
- 省去了proto文件编写，直接用C#接口来定义服务，得益于：[protobuf-net.Grpc](https://github.com/protobuf-net/protobuf-net.Grpc)。
