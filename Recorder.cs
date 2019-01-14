using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fourierResult = System.Collections.Generic.List<fourierValue>;
using System.Linq;
using System.Threading;

public class Recorder : MonoBehaviour {
    // Start is called before the first frame update
    public FourieView view;

    private
    void Start () {
        AudioSource source = gameObject.AddComponent<AudioSource> ();
        AudioClip clip = Microphone.Start (null, true, 1, 44100);
        source.clip = clip;
        source.loop = true;
        while (Microphone.GetPosition (null) < 0) {

        }

        source.Play ();
    }

    // Update is called once per frame
    void Update () {

    }

    void OnAudioFilterRead (float[] data, int channels) {
        // 擬似コード
        //録音(data);
        int i = 0;
        // 出力をミュートする
        for (i = 0; i < data.Length; i++) {
            data[i] = data[i] * 1;
        }

        //Functions.Performance (() => {

        new Thread (() => {
            int appended=0;
            foreach (fourierResult result in Fourier.FastFourier (
                    //制度調整を
                    data.Where ((f, index) => index % 4 ==
                        0).ToArray (), 256)) {
                if (view) {
                    view.PushResult (result);
                    appended++;
                }
            }
            //Debug.Log($"Add {appended}");    
        }).Start ();

        //});

    }
}