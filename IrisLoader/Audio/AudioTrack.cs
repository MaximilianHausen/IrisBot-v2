﻿using System;
using System.IO;

namespace IrisLoader.Audio;

public class AudioTrack : IDisposable
{
    public event Action PlaythoughFinished;

    internal readonly Stream stream;

    public AudioTrack(Stream data) => stream = data;

    ~AudioTrack()
    {
        Dispose();
    }

    public void Dispose() => stream.Dispose();

    public void SetPosition(TimeSpan position) => stream.Seek((int)position.TotalMilliseconds * 192, SeekOrigin.Begin);

    internal void UpdateFinishEvent()
    {
        if (stream.Length <= stream.Position)
            PlaythoughFinished();
    }
}
