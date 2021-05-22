using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Tex2imgSettingGenerator
{
    class Program
    {
        const string SettingFileName = "SettingByGenerator.tex";
        const string Tex2img = "TeX2imgc.exe";
        static void Main(string[] args)
        {
            if (!new FileInfo(Tex2img).Exists)
            {
                Console.WriteLine("Tex2imgが存在しません キーを押してウィンドウを閉じます");
                Console.ReadKey();
                return;
            }
            var logDir = new DirectoryInfo("log");
            if (!logDir.Exists)
                logDir.Create();
            StringBuilder logText = new StringBuilder();
            foreach (var f in new DirectoryInfo("./").GetFiles().Where(x => x.Extension == ".tex"))
            {
                if (f.Name != SettingFileName)
                {
                    var s = String.Format(@"\documentclass[fleqn,papersize,dvipdfmx]{{jarticle}}
\usepackage{{amsmath,amssymb}}
\usepackage{{color}}
\pagestyle{{empty}}
\begin{{document}}
\input{{{0}}}
\end{{document}}", f.Name);
                    File.WriteAllText(SettingFileName, s);
                    var Names = new string[2];
                    Names[0] = f.Name.Substring(0, f.Name.Length - ".tex".Length) + ".png";
                    Names[1] = f.Name.Substring(0, f.Name.Length - ".tex".Length) + ".svg";
                    foreach (var n in Names)
                    {
                        var psInfo = new ProcessStartInfo
                        {
                            FileName = Tex2img, // 実行するファイル
                            Arguments = String.Format("{0} {1}", SettingFileName, n),
                            CreateNoWindow = true, // コンソール・ウィンドウを開かない
                            UseShellExecute = false, // シェル機能を使用しない
                            RedirectStandardOutput = true // 標準出力をリダイレクト
                        };

                        Console.WriteLine(n + "を生成中...");
                        Process p = Process.Start(psInfo); // アプリの実行開始
                        string output = p.StandardOutput.ReadToEnd(); // 標準出力の読み取り

                        output = output.Replace("\r\r\n", "\n"); // 改行コードの修正
                        Console.Write(output); // ［出力］ウィンドウに出力
                        logText.Append(output);
                    }
                }
            }
            var logName = Path.Combine(logDir.FullName, DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss"))+ ".txt";
            /*File.Create(logName);
            File.
            */File.WriteAllText(logName, logText.ToString());
            Console.WriteLine("完了しました");
            Console.WriteLine("キーを押して終了します");
            Console.ReadKey();
        }
    }
}
