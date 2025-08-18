
using System;

namespace FriteCollection2.Tools.Particles;

public interface IParticle<Settings> : IDisposable, IDraw
{
    public void Initialize(in Settings settings);
    public void Update();
    public bool Alive { get; }
}

public class ParticleGenerator<P, Sets> : IDraw, IDisposable where P : IParticle<Sets>, new()
{
    private readonly float delay;
    private P[] _data;

    public P this[int index] => _data[index];
    public int Capacity => _data.Length;

    private ushort index;
    private float timer;

    private bool _isEmpty;
    public bool IsEmpty => _isEmpty;

    public ParticleGenerator(ushort capacity, ushort pps)
    {
        this.delay = 60f / pps;
        _data = new P[capacity];
        for (ushort i = 0; i < capacity; ++i)
            _data[i] = new();
        index = 0;
        timer = delay;
        _isEmpty = true;
    }

    public void Charboner(in Sets settings)
    {
        timer += Time.FrameTime;
        while (timer > delay)
        {
            timer -= delay;
            Create(in settings);
        }
    }

    public void Create(in Sets settings)
    {
        if (!_data[index].Alive)
        {
            _data[index].Initialize(in settings);
            ++index;
            if (index >= _data.Length)
                index = 0;
        }
    }

    public void Update()
    {
        _isEmpty = true;
        for (ushort i = 0; i < _data.Length; ++i)
        {
            if (_data[i].Alive)
            {
                _data[i].Update();
                if (_data[i].Alive)
                    _isEmpty = false;
            }
        }
    }

    public void Draw()
    {
        for (ushort i = 0; i < _data.Length; ++i)
        {
            if (_data[i].Alive)
                _data[i].Draw();
        }
    }

    public void Dispose()
    {
        for (ushort i = 0; i < _data.Length; ++i)
        {
            _data[i].Dispose();
        }
        _data = null;
    }
}