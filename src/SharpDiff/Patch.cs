﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpDiff.Core;
using SharpDiff.FileStructure;

namespace SharpDiff
{
    public class Patch
    {
        private readonly Diff diff;
        public IFileAccessor File { get; set;}

        public Patch(Diff diff)
        {
            this.diff = diff;
            File = new FileAccessor();
        }

        public string ApplyTo(string path)
        {
            var fileContents = File.ReadAll(path);
            var fileLines = new List<string>(fileContents.Split(new[] {"\r\n"}, StringSplitOptions.None));

            foreach (var chunk in diff.Chunks)
            {
                var lineLocation = chunk.NewRange.StartLine - 1; // zero-index the start line 

                if (lineLocation < 0)
                    lineLocation = 0;

                int contextlines = 0;
                foreach (var snippet in chunk.Snippets)
                {
                    if(!(snippet is ContextSnippet))
                    {
                        if (contextlines > 0)
                            lineLocation += contextlines; //TODO
                    }
                    if (snippet is AdditionSnippet)
                    {
                        foreach (var line in snippet.ModifiedLines)
                        {
                            fileLines.Insert(lineLocation, line.Value);
                            lineLocation++;
                        }
                    }
                    else if (snippet is SubtractionSnippet)
                    {
                        foreach (var line in snippet.OriginalLines)
                        {
                            fileLines.RemoveAt(lineLocation);
                        }
                    }
                    else if (snippet is ContextSnippet)
                    {
                        foreach (string line in snippet.OriginalLines.Select(l => l.Value))
                        {
                            bool matches = true; //Only add if all lines match in snippet - But there are separate snippets for each line
                            for (int i = -2; i <= 2; i++)
                            {
                                if (!(lineLocation + i > 0 && lineLocation + i < fileLines.Count && line == fileLines[lineLocation + i]))
                                {
                                    matches = false;
                                    break;
                                }
                            }
                            if (matches) //TODO: Test
                                contextlines += snippet.OriginalLines.Count();
                        }
                        //lineLocation += snippet.OriginalLines.Count();
                    }
                }
            }

            return string.Join("\r\n", fileLines.ToArray());
        }
    }
}