using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    // 保存重命名的历史记录
    static List<(string oldName, string newName)> history = new List<(string, string)>();

    static void Main(string[] args)
    {
        Console.WriteLine("欢迎使用文件批量重命名工具！");
        Console.WriteLine("此工具支持批量重命名文件，并提供预览、撤回操作等功能。");
        Console.WriteLine("使用提示：");
        Console.WriteLine("1. 输入文件夹路径。");
        Console.WriteLine("2. 选择操作类型，如添加前缀、后缀、替换字符等。");
        Console.WriteLine("3. 预览修改效果，并确认是否执行。");

        while (true)
        {
            Console.WriteLine("\n请选择操作:");
            Console.WriteLine("1. 批量重命名文件");
            Console.WriteLine("2. 撤回上一次操作");
            Console.WriteLine("3. 查看历史记录");
            Console.WriteLine("4. 退出程序");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        BatchRenameFiles();
                        break;
                    case 2:
                        UndoLastAction();
                        break;
                    case 3:
                        ShowHistory();
                        break;
                    case 4:
                        Console.WriteLine("感谢使用本工具！再见！");
                        return;
                    default:
                        Console.WriteLine("无效的选择，请重新选择！");
                        break;
                }
            }
            else
            {
                Console.WriteLine("无效的输入，请重新选择！");
            }
        }
    }

    static void BatchRenameFiles()
    {
        Console.WriteLine("请输入目标文件夹路径:");
        string directoryPath = Console.ReadLine();

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("指定的目录不存在。");
            return;
        }

        Console.WriteLine("请选择重命名规则:");
        Console.WriteLine("1. 添加前缀");
        Console.WriteLine("2. 添加后缀");
        Console.WriteLine("3. 替换字符");
        int choice = int.Parse(Console.ReadLine());

        string[] files = Directory.GetFiles(directoryPath);
        string[] previewFiles = new string[files.Length];

        switch (choice)
        {
            case 1:
                Console.WriteLine("请输入要添加的前缀:");
                string prefix = Console.ReadLine();
                previewFiles = files.Select(f => Path.Combine(directoryPath, prefix + Path.GetFileName(f))).ToArray();
                break;
            case 2:
                Console.WriteLine("请输入要添加的后缀:");
                string suffix = Console.ReadLine();
                previewFiles = files.Select(f => Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(f) + suffix + Path.GetExtension(f))).ToArray();
                break;
            case 3:
                Console.WriteLine("请输入要替换的字符:");
                string oldChar = Console.ReadLine();
                Console.WriteLine("请输入替换后的字符:");
                string newChar = Console.ReadLine();
                previewFiles = files.Select(f => Path.Combine(directoryPath, Path.GetFileName(f).Replace(oldChar, newChar))).ToArray();
                break;
            default:
                Console.WriteLine("无效的选择。");
                return;
        }

        Console.WriteLine("预览修改效果:");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{Path.GetFileName(files[i])} => {Path.GetFileName(previewFiles[i])}");
        }

        Console.WriteLine("是否确认修改？(y/n):");
        string confirmation = Console.ReadLine();
        if (confirmation.ToLower() == "y")
        {
            for (int i = 0; i < files.Length; i++)
            {
                string newFileName = previewFiles[i];
                if (newFileName != files[i])
                {
                    File.Move(files[i], newFileName);
                    Console.WriteLine($"已将 '{Path.GetFileName(files[i])}' 重命名为 '{Path.GetFileName(newFileName)}'");
                    // 记录历史操作
                    history.Add((files[i], newFileName));
                }
            }
            Console.WriteLine("重命名操作完成。");
        }
        else
        {
            Console.WriteLine("操作已取消。");
        }
    }

    static void UndoLastAction()
    {
        if (history.Count == 0)
        {
            Console.WriteLine("没有可以撤回的操作。");
            return;
        }

        var lastAction = history.Last();
        Console.WriteLine($"撤回操作: 将 '{Path.GetFileName(lastAction.newName)}' 恢复为 '{Path.GetFileName(lastAction.oldName)}'");
        File.Move(lastAction.newName, lastAction.oldName);
        history.RemoveAt(history.Count - 1); // 移除最后一次的历史记录
        Console.WriteLine("撤回操作已完成。");
    }

    static void ShowHistory()
    {
        if (history.Count == 0)
        {
            Console.WriteLine("历史记录为空。");
            return;
        }

        Console.WriteLine("历史记录:");
        foreach (var item in history)
        {
            Console.WriteLine($"{Path.GetFileName(item.oldName)} => {Path.GetFileName(item.newName)}");
        }
    }
}
