# **select-lucky-dog**
~~选择幸运狗~~只是一个抽人的软件罢了
## 甲方提出的&未完成的功能
+ 跳过标记状态
+ 优化数据集导入算法
+ 导入导出用户数据
+ “只抽数字”
+ 生成日志
+ 退出时保存
+ 自动保存
+ 优化内存占用
## 版本
（版本号格式为：大更新数.发布次数.解决问题or添加功能总数.交给微软管理数）
+ 3.5.vNext.beta
  + 添加功能
    + 显示各个类别学生总数
	+ 标记状态下显示未完成学生占比
	+ 添加抽人动画
	+ 添加退出预览模式后的操作指引
	+ 新的显示效果：Acrylic（附加切换按钮）
  + 完善功能
    + 完善软件错误信息
	+ 调整部分控件的外观
	+ 使显示效果覆盖到标题栏
	+ 解决了一个可能导致应用闪退的问题
  + 添加预览功能
    + 导出
	  + 身份文件
	+ 退出时提醒保存
  + 删除预览功能
    + 导出
	  + 用户数据
	  + 日志文件
  + 完善预览功能
    + 清理
      + 导入的数据集
      + 用户数据
  	+ 解决了一个导致未按照预期标记的问题
	+ 解决了一个可能导致保存不完整的问题
	+ 解决了无法导出重名的人的问题
	+ 解决了搜索框没有正常工作的问题
	+ 减少了导出时所需时间
  + 正式发布功能
    + 搜索学生
	+ 分类学生列表
	+ “导出”按钮的所有功能
	+ “操作”按钮的所有功能
	+ 退出时提醒保存功能

+ 3.4.28.0.beta.220527-1000
  + 完善预览功能
    + 完善了部分功能的中文提示
	+ 添加部分功能的身份验证
  + 完善功能
    + 完善的软件的稳定性
	+ 修复了软件版本显示错误的问题
  + 添加功能
    + 捕获全局异常并可选发送邮件
	+ 拖动并导入数据集
	+ 身份验证通过提示
  + 添加预览按钮
    + 导出身份文件

+ 3.3.21.0.beta.220524-2155
  + 更新用户控件集至WinUI 2.7.1
  + 添加预览体验模式警告
    + 添加退出预览模式功能
    + 添加反馈问题功能
  + 添加功能
    + 记住上次的标记状态
    + 在标记模式下显示抽过的人
  + 完善功能
    + 添加确认标记窗口的默认选择
    + 添加确认体验预览模式窗口的默认选择 
    + 修复了单次运行中无法二次调出预览模式对话窗的问题
    + 完善选择与导入数据集完成的提示
    + 更改数据集导入格式（※不兼容的更改）
    + 调整部分控件的位置
  + 添加预览控件
    + 导出
      + 数据集（完成功能）
      + 日志
      + 用户数据
    + 清理
      + 导入的数据集
      + 日志
      + 用户数据
    + 操作
      + 标记（完成功能）
	  + 标记未完成（完成功能）
	  + 保存（完成功能）
    + 查询学生并显示其状态（完成功能）
    + 显示学生（完成功能）
           + 所有
            +  进行中
            +  已完成
            +  未完成
  + 移除预览按钮
    + 导出数据
+ 2.2.10.0.beta.220427-2203
  + 完成“标记”功能
  + 添加“打开时读取上次成功导入的数据集”功能
  + 添加预览按钮
    + 导出数据
+ 2.1.7.0.prebeta.220424-2138
  + 添加显示导入数据进度条
  + 添加预览模式
  + 重写用户界面
  + 添加版本号显示
+ 1.0.4.0.alpha.220415-2200
  + 添加随机选人功能
  + 添加读取并导入数据集功能
  + 添加无用的“表扬一下”功能
  + 添加“标记”按钮（未实现功能）