将两个.bat文件放到要打包的工程目录下

Auto是打包并上传nuget包
Pack是打包生成.nupkg文件
Push是上传到jtext103的nuget服务器

上传会上传最大版本，但是可能不靠谱，最好保持工程目录下只有一个.nupkg文件