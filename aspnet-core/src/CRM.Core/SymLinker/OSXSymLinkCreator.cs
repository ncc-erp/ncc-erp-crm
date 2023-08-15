using System;

namespace CRM.SymLinker
{
    public class OSXSymLinkCreator : ISymLinkCreator
    {
        public bool CreateSymLink(string linkPath, string targetPath, bool file)
        {
            throw new NotImplementedException("OSXSymLinkCreator");
        }
    }
}
