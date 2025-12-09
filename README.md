# AnnouncementCore

一个简单的，快速的无依赖，无配置，无冲突的
快捷极简Mod公告系统
![演示图](https://raw.githubusercontent.com/FIKRY666/AnnouncementCore/main/%E6%BC%94%E7%A4%BA%E5%9B%BE.png)
用户会在初次装载Mod的时候自动弹出公告，会在初次关闭的时候存储状态，直到版本号更新

极简的使用方法，扔进去即可

## 目录结构
```
YourMod/
├── announcement_config.json       # 本Mod配置
├── changelog.md                   # 更新说明
├── YourMod.dll                    # 你的Mod
└── AnnouncementCore.dll           # 本Mod
```

## 使用方法

### 1. 在编译的时候引用项目

### 2. 在Mod主文件中添加两行代码
```csharp
private void OnEnable()
{
    AnnouncementManager.WakeUp();  // 起床
}

private void OnDisable()
{
    AnnouncementManager.CleanupMod("YourModID");  // 注销
}
```

**简单粗暴，不需要任何额外操作**

## 不用担心其他mod也使用本Mod导致的冲突问题

## announcement_config.json
没啥麻烦的内容
```json
{
    // Mod唯一标识符（英文/数字，无空格可使用符号但是不建议使用除".","_"之外的符号）
    "ModId": "MyAwesomeMod",
    
    // 显示名称（支持中文，显示在按钮和标题）
    "DisplayName": "我的Mod",
    
    // 按钮文字（简短描述）
    "ButtonText": "骗进来杀",
    
    // 版本号（语义化版本，检测更新用）
    "Version": "1.2.3",
    
    // 作者（可选）
    "Author": "你也可以写我😋",
    
    // 创意工坊ID（可选，Steam Workshop ID）
    "SteamWorkshopId": "1234567890",
    
    // 按钮颜色（可选，十六进制RGBA，骗进来杀的颜色）
    "ButtonColor": "#2B5079BF",
    
    // 自动显示介绍（可选，首次安装时）
    "AutoShowIntro": true,
    
    // 自动显示更新（可选，版本变化时）
    "AutoShowUpdates": true
}
```

## changelog.md 
没啥限制，纯文本，想写啥写啥

完整支持Unity的富文本嗯对，不过最好别写太长，因为滚动是有上限的🥱

```markdown
# <color=#FF6B6B>v1.2.3 更新内容</color>

## <b>新功能</b>
- <color=green>添加了XXX系统</color>
- <i>支持YYY物品</i>
- <size=16>新增ZZZ地图</size>

## <color=orange>问题修复</color>
- 修复了崩溃问题
- 修正了显示错误
```

---

**感谢各位大佬耐心看完，感谢各位大佬给我当小白鼠，我自己也就测了我自己的两个mod**
