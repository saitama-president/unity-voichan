using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using fourierResult = System.Collections.Generic.List<fourierValue>;
public class FourieView : MonoBehaviour {
  public int BandsCount = 256;
  public delegate void FourieFrameData (fourierResult v);
  private List<LineRenderer> bands = new List<LineRenderer> ();

  public event FourieFrameData OnUpdateFourieView;

  private List<fourierResult> fourieBuff = new List<fourierResult> ();

  void Awake () {
    foreach (Transform t in this.transform) {
      GameObject.Destroy (t.gameObject);
    }
    Functions.For (this.BandsCount, index => {
      GameObject o = new GameObject (index.ToString ());
      bands.Add (o.AddComponent<LineRenderer> ());

      o.transform.SetParent (this.transform);

    });
    this.OnUpdateFourieView += doOnUpdateView;
  }

  private void doOnUpdateView (fourierResult v) { }

  public void PushResult (fourierResult result) {
    this.fourieBuff.Add (result);
  }

  void Start () {

  }

  // Update is called once per frame
  void Update () {
    Functions.TimeLimitProcAndShift (
      fourieBuff,
      o => {
        //ここに処理を書く
        for (int i = 0; i < this.BandsCount; i++) {

          bands[i].positionCount++;
          //bands[]
        }
      }
    );
  }

  
}