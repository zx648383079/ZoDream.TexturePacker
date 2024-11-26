using PsdParser;
using System;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Project.Adobe
{
    public class PsdReader : IProjectReader
    {
        public Task<ProjectDocument?> ReadAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                try
                {
                    return Read(fileName);
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        private ProjectDocument Read(string fileName)
        {
            using var reader = new PsdFile(fileName);
            var document = new ProjectDocument();
            
            return document;
        }


        public Task WriteAsync(string fileName, ProjectDocument data)
        {
            throw new NotImplementedException();
        }

    }
}
