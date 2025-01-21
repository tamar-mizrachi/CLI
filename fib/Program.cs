using System.CommandLine;


var bundleCommand = new Command("bundle", "Bundle code file to a single file.");
var createRspCommand = new Command("create-rsp", "Create a response file for the bundle command");

var outputOption = new Option<FileInfo>("--output", "file path and name.");
outputOption.AddAlias("-o");
var languageOption = new Option<string[]>("--language", "Programming language to include (java, c#,css,html,python, javascript). Use 'all' for all files.")
{
    IsRequired = true
};
languageOption.AddAlias("-l");
var noteOption = new Option<bool>("--note", "if write the code in note ");
noteOption.AddAlias("-n");
var sortOption = new Option<string>("--sort", getDefaultValue: () => "abc", "sort by?");
sortOption.AddAlias("-s");
var authorOption = new Option<string>("--author", "name of own code");
authorOption.AddAlias("-a");
var removeEmptyLinesOption = new Option<bool>("--remove-empty-lines", "Remove empty lines from source code");
removeEmptyLinesOption.AddAlias("r");
bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(authorOption);
bundleCommand.AddOption(removeEmptyLinesOption);

createRspCommand.AddOption(outputOption);

bundleCommand.SetHandler((output, language, note, sort, author, removeEmptyLines) =>
{
    try
    {
        List<string> fileNames = Directory.GetFiles(".\\", "*", SearchOption.AllDirectories).ToList();

        if (language[0] != "all")
        {
            List<string> l = new List<string>();
            for (int i = 0; i < language.Length; i++)
            {
                switch (language[i])
                {
                    case "java"or "Java":
                        { 
                            l.AddRange(fileNames.Where(x => x.EndsWith(".java"))); break;}
                    case "c#"or "C#":
                        { l.AddRange(fileNames.Where(x => x.EndsWith(".cs"))); break; }
                    case "css"or "Css":
                        {
                            l.AddRange(fileNames.Where(x => x.EndsWith(".css")));
                            break;
                        }

                    case "html"or "Html":
                    {
                            l.AddRange(fileNames.Where(x => x.EndsWith(".html")));
                            break;
                    }

                    case "python"or "Python":
                    {
                            l.AddRange(fileNames.Where(x => x.EndsWith(".py")));
                            break;
                    }

                    case "javascript"or"Javascript":
                        {
                            l.AddRange(fileNames.Where(x => x.EndsWith(".js")));
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("its not identified language" + language[i]);
                            break;
                        }
                };
            }
            fileNames = l;

        }
        if (sort == "abc") { fileNames = fileNames.OrderBy(x => x).ToList(); }
        if (sort == "language") { fileNames = fileNames.OrderBy(x => Path.GetExtension(x)).ToList(); }
        using (var file = File.CreateText(output.FullName))
        {
            Console.WriteLine("file was created with name: " + output.FullName);
            if (author != null)
            { file.WriteLine("author: " + author); }
            string[] contiens;
            for (int i = 0; i < fileNames.Count; i++)
            {
                if (note)
                    file.WriteLine("file name: " + Path.GetFileName(fileNames[i]));
                file.WriteLine("file path: " + fileNames[i]);
                file.WriteLine("file content:");
                contiens = File.ReadAllLines(fileNames[i]);
                if (removeEmptyLines)
                {
                    contiens = contiens.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                }
                file.WriteLine(string.Join("\n", contiens));
            }
        }

    }
    catch (Exception ex)

    {
        Console.WriteLine("couldnt create the file");
    }

}, outputOption, languageOption, noteOption, sortOption, authorOption, removeEmptyLinesOption);

var rootCommand = new RootCommand("root command for file bundle cli");
rootCommand.AddCommand(bundleCommand);
createRspCommand.SetHandler(() =>
{
    try
    {
        var responseFile = new FileInfo("response.rsp");
        Console.WriteLine("enter values to  the bundle command");
        using (var file = File.CreateText("responseFile.rsp"))
        {
            Console.WriteLine("output file path: ");
            var output1 = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(output1)){
                Console.WriteLine("enter the output file path:");
                output1 = Console.ReadLine();
            }
            file.WriteLine("--output" + output1);
            Console.Write("enter the language :");
            Console.Write("languages(comma-separated) :");
            var languages = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(languages))
            {
                Console.WriteLine("enter at least one programming language:");
                languages = Console.ReadLine();
            }
            file.WriteLine("--language" + languages);
            Console.Write("include notes?(y/n):");
            var note = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(note) || (note != "y" && note != "n"))
            {
                Console.Write("enter 'y' or 'n': ");
                note = Console.ReadLine();
            }
            if (note == "y")
                file.WriteLine("--note");
            Console.Write("sort by abc/language:");
            var sort = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(sort) || (sort != "abc" && sort != "language"))
            {
                Console.WriteLine("enter 'abc' or 'language':");
                sort = Console.ReadLine();
            }
            file.WriteLine("--sort" + sort);
            Console.WriteLine("remove empty lines?(y/n):");
            var remove = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(remove) || (remove != "y" && remove != "n"))
            {
                Console.WriteLine("enter 'y' or 'n':");
                remove = Console.ReadLine();
            }
            if (remove == "y")
                file.WriteLine("--remove-empty-lines" + remove);
            Console.Write("enter name of author:");
            var author = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(author))
            {
                file.WriteLine("--author" + author);
            }
            Console.WriteLine("response file was created good:" + responseFile.FullName);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("created the response file was faild");
    }

});
rootCommand.AddCommand(createRspCommand);
await rootCommand.InvokeAsync(args);



