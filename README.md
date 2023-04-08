# DesktopQRScanner Windows二维码扫描小工具

![图片](https://user-images.githubusercontent.com/8372598/230618941-ff44320a-5ae4-4ff5-b3bb-1b92b1624bc9.png)

本项目使用.NET6.0开发，集成二维码扫描、历史记录管理、二维码重新生成等功能。无广告，不联网。

使用方法：

左侧图片展示区域中部“+”按钮可选择图片文件或使用“截图”按钮截取电脑屏幕以读取二维码。

右侧列表区域提供简单的管理功能。

本项目引用CommunityToolkit.Mvvm、HandyControl、Newtonsoft.Json、ZXing.Net、HinsHo.ScreenShot.CSharp，感谢这些开源库及其作者们的辛勤付出。

本项目的HandyControl样式库存在自定义，请参照<https://github.com/GSCmax/HandyControl>。

编译方法：

1、克隆<https://github.com/GSCmax/DesktopQRScanner>，使用VS2022打开项目，等待VS还原Nuget程序包；

2、克隆<https://github.com/GSCmax/HandyControl>，使用VS2022编译Release，复制HandyControl\src\Net_GE45\HandyControl_Net_GE45\bin\Release\net6.0-windows下除文件夹之外的四个文件至C:\Users\你的用户名\.nuget\packages\handycontrol\3.4.0\lib\net6.0\下，替换原有项目；

3、编译本项目。
