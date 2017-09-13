#pragma strict
var sound1: AudioClip; // drag the sound 1 here
var sound2: AudioClip; // drag the sound 2 here

function Start () {

}

function Update () {


if(Input.GetButtonDown("Vertical")){

    //audio.PlayOneShot(sound1);
    GetComponent.<AudioSource>().loop = true; GetComponent.<AudioSource>().clip = sound1; GetComponent.<AudioSource>().Play();
}
if(Input.GetButtonUp("Vertical")){

    //audio.PlayOneShot(sound1);
    GetComponent.<AudioSource>().Stop(); 
}


}