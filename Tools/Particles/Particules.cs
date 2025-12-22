using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection2.Tools.Particles;

public interface IParticle<Settings> : IDisposable, IDraw
{
    public void Initialize(in Settings settings);
    public void Update(float dt);
    public bool Alive { get; }
}

public class ParticleGenerator<P, Sets> : IDraw, IDisposable where P : IParticle<Sets>, new()
{
    private float delay;
    private P[] _data;

    public P this[int index] => _data[index];
    public int Capacity => _data.Length;

    private ushort index;
    private float timer;

    private bool _isEmpty;
    public bool IsEmpty => _isEmpty;

    private Sets settings;

    public ParticleGenerator(ushort capacity, ushort pps, in Sets settings)
    {
        this.delay = 1f / pps;
        _data = new P[capacity];
        for (ushort i = 0; i < capacity; ++i)
            _data[i] = new();
        index = 0;
        timer = delay;
        _isEmpty = true;
        this.settings = settings;
    }

    public void Charboner(float dt)
    {
        timer += dt;
        while (timer > delay)
        {
            timer -= delay;
            Create();
        }
    }

    public void ChangePPS(ushort pps)
    {
        delay = 1f / pps;
    }


    public void ChangePPS(int pps)
    {
        delay = 1f / pps;
    }

    public void Create()
    {
        if (!_data[index].Alive)
        {
            _data[index].Initialize(in settings);
            ++index;
            if (index >= _data.Length)
                index = 0;
        }
    }

    public void Update(float dt)
    {
        _isEmpty = true;
        for (ushort i = 0; i < _data.Length; ++i)
        {
            if (_data[i].Alive)
            {
                _data[i].Update(dt);
                if (_data[i].Alive)
                    _isEmpty = false;
            }
        }
    }

    public void Draw(in SpriteBatch batch)
    {
        for (ushort i = 0; i < _data.Length; ++i)
        {
            if (_data[i].Alive)
                _data[i].Draw(in batch);
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