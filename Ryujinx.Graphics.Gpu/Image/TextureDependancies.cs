using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ryujinx.Graphics.Gpu.Image
{
    class TextureSubReference
    {
        public Texture Texture;
        public int FirstLayer;
        public int FirstLevel;
    }

    class TextureDependancy
    {
        public TextureSubReference From;
        public TextureSubReference To;

        public TextureDependancies Target => To.Texture.Dependancies;

        public void Copy()
        {
            //From.Texture.TrySynchronizeMemory();
            From.Texture.HostTexture.CopyTo(To.Texture.HostTexture, From.FirstLayer, To.FirstLayer, From.FirstLevel, To.FirstLevel);
        }
    }

    class TextureDependancies: IDisposable
    {
        public bool Dirty { get; private set; }

        private Texture _parent;
        private TextureDependancy _lastModified;

        public List<TextureDependancy> Dependancies { get; }
        public List<TextureDependancy> ModifiedQueue { get; } // This is a list for easier management.

        public TextureDependancies(Texture parent)
        {
            Dependancies = new List<TextureDependancy>();
            ModifiedQueue = new List<TextureDependancy>();

            _parent = parent;
        }

        public void AddOneWayDependancy(TextureSubReference from, TextureSubReference to)
        {
            Dependancies.Add(new TextureDependancy
            {
                From = from,
                To = to
            });
        }

        public void Modified()
        {
            foreach (TextureDependancy dependancy in Dependancies)
            {
                dependancy.Target.DependantModified(dependancy);
            }
        }

        public void Modified(Texture target)
        {
            foreach (TextureDependancy dependancy in Dependancies.Where(dependancy => dependancy.To.Texture == target))
            {
                dependancy.Target.DependantModified(dependancy);
            }
        }

        public void Modified(int firstLayer, int firstLevel, HashSet<Texture> ignoreSet)
        {
            foreach (TextureDependancy dependancy in Dependancies)
            {
                if (dependancy.From.FirstLayer == firstLayer && dependancy.From.FirstLevel == firstLevel && !ignoreSet.Contains(dependancy.To.Texture))
                {
                    dependancy.Target.DependantModified(dependancy, ignoreSet);
                }
            }
        }

        private void DependantModified(TextureDependancy other, HashSet<Texture> ignoreSet = null)
        {   
            // Quick optimization: we don't have to do any work if the dependancy is already at the top of the modified queue.
            other.From.Texture.TrySynchronizeMemory();
            other.To.Texture.ConsumeModified();
            other.Copy();

            if (ignoreSet == null)
            {
                ignoreSet = new HashSet<Texture>();
            }
            ignoreSet.Add(other.From.Texture);

            other.To.Texture.Dependancies.Modified(other.To.FirstLayer, other.To.FirstLevel, ignoreSet);

            /*
            if (_lastModified != other)
            {
                _parent.SynchronizeMemoryNoDependants();

                ModifiedQueue.Remove(other); // It may exist at a place that isn't at the end of the queue, remove it so it only appears once.
                ModifiedQueue.Add(other);
                _lastModified = other;

                Dirty = true;
                _parent.SignalDirty();
            }
            */
        }

        public void ClearModified()
        {
            ModifiedQueue.Clear();
            _lastModified = null;
        }

        public TextureDependancy FindDependancy(Texture from, Texture to)
        {
            return Dependancies.FirstOrDefault(dep => dep.From.Texture == from && dep.To.Texture == to);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Synchronize()
        {
            if (Dirty)
            {
                foreach (var modified in ModifiedQueue)
                {
                    modified.Copy();
                }

                ClearModified();
                Dirty = false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Synchronize this texture if the given texture is part of its modified queue. (eg. it is being deleted after being modified, and needs to push changes to us)
        /// </summary>
        /// <param name="texture">The texture to look for</param>
        /// <returns></returns>
        public bool Synchronize(Texture texture)
        {
            if (Dirty)
            {
                if (ModifiedQueue.Any(x => x.From.Texture == texture))
                {
                    return Synchronize();
                }
            }

            return false;
        }

        public void RemoveTexture(Texture texture)
        {
            Dependancies.RemoveAll(dependancy => dependancy.To.Texture == texture);
        }

        public void Dispose()
        {
            foreach (TextureDependancy dependancy in Dependancies)
            {
                dependancy.To.Texture.Dependancies.RemoveTexture(_parent);
            }
        }
    }
}
