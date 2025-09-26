using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource _sourceDead;
    [SerializeField] AudioSource _sourceGrab;
    [SerializeField] AudioSource _sourceJump;
    [SerializeField] AudioSource _sourceThrow;
    [SerializeField] AudioSource _sourceSpawn;
    [SerializeField] AudioSource _sourceBumper;
    [SerializeField] AudioSource _sourceWalk;

    public void Dead()
    {
            _sourceDead.Play();
    }

    public void Grab()
    {
            _sourceGrab.Play();
    }

    public void Jump()
    {
            _sourceJump.Play();
    }

    public void Throw()
    {
            _sourceThrow.Play();
    }

    public void Spawn()
    {
            _sourceSpawn.Play();
    }

    public void Bumper()
    {
            _sourceBumper.Play();
    }

    public void Walk(bool value)
    {
        if (!value)
        {
            _sourceWalk.Stop();
            return;
        }

        if (!IsPlaying(_sourceWalk))
            _sourceWalk.Play();
    }

    public bool IsPlaying(AudioSource source)
    {
        if (source.isPlaying)
            return true;
        else
            return false;
    }
}
