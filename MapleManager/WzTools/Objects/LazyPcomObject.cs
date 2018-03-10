﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleManager.WzTools.Objects
{
    class LazyPcomObject<T> : PcomObject where T : PcomObject, new()
    {
        private BinaryReader lazyReader;
        public bool Loaded { get; set; } = false;
        private PcomObject _actualObject = null;
        public int OffsetInFile { get; set; }

        public LazyPcomObject(BinaryReader reader)
        {
            lazyReader = reader;
            OffsetInFile = (int)reader.BaseStream.Length;
        }

        public override void Init(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        private void TryLoad()
        {
            if (Loaded) return;
            _actualObject = new T();
            lazyReader.JumpAndReturn(OffsetInFile, () => _actualObject.Init(lazyReader));
            lazyReader = null;
            Loaded = true;
        }

        public override void Set(string key, object value)
        {
            TryLoad();
            _actualObject.Set(key, value);
        }

        public override object Get(string key)
        {
            TryLoad();
            return _actualObject.Get(key);
        }

        public override void Rename(string key, string newName)
        {
            TryLoad();
            _actualObject.Rename(key, newName);
        }
    }
}
