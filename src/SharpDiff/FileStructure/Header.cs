using System;
using System.Collections.Generic;
using SharpDiff.FileStructure;

namespace SharpDiff.FileStructure
{
    public class Header
    {
        public Header(FormatType format)
            : this(format, new List<IFile>())
        {}

        public Header(FormatType format, IEnumerable<IFile> files)
        {
            Format = format;
            if (files != null)
                Files = new List<IFile>(files);
        }

        public FormatType Format { get; private set; }
        public IList<IFile> Files { get; private set; }

        public virtual bool IsNewFile
        {
            get { return Files[0] is NullFile; }
            protected set { }
        }

        public virtual bool IsDeletion
        {
            get { return Files[1] is NullFile; }
            protected set { }
        }
    }
}