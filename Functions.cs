using System.Collections;
using System.Collections.Generic;
using fourierResult = System.Collections.Generic.List<fourierValue>;
using System;
using System.Linq;
using UnityEngine;

public class Functions {
  public delegate void Action ();

  public delegate void indexFunc (int index);
  public delegate void Vsync (int y);

  public delegate void CellFunc (int x, int y);
  public delegate void MicroTask<T> (T obj);

  public static void For (int count, indexFunc func) {
    for (int i = 0; i < count; i++) {
      func (i);
    }
  }

  /**
    制限時間内に処理を終わらせる
    （終わらなかったら強制終了）
   */
  public static void TimeLimitProcAndShift<T> (

    List<T> list,
    MicroTask<T> task,
    int millisec = 10
  ) {
    System.Diagnostics.Stopwatch watch =
      new System.Diagnostics.Stopwatch ();
    watch.Start ();

    int i = 0;
    int count=list.Count;
    list.RemoveAll (
      o => {
        if (millisec < watch.Elapsed.Milliseconds) {
          return false;
        }
        task (o);
        i++;
        return true;
      }
    );
    Debug.Log ($"PROCEED={i} / {count}  ");
  }

  public static void Performance (Action act) {
    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
    watch.Start ();
    act ();
    watch.Stop ();

    Debug.Log (watch.Elapsed.ToString ("s'.'fffff"));
  }

  public static void SquareWalk<T> (
    T[] input,
    Vsync begin,
    CellFunc cellFunc,
    Vsync end
  ) {
    for (int y = 0; y < input.Length; y++) {
      begin (y);
      for (int x = 0; x < input.Length; x++) {
        cellFunc (x, y);
      }
      end (y);
    }
  }

  public static void FoldWalk<T> (
    T[] input,
    int foldSize,
    Vsync begin,
    CellFunc cellFunc,
    Vsync end
  ) {
    for (int i = 0, Y = 0; i < input.Length; i += foldSize,
      Y++) {
      begin (Y);
      for (var X = 0; X < foldSize; X++) {
        cellFunc (X, Y);
      }
      end (Y);
    }
  }
}

public struct fourierValue {
  public fourierValue (float real, float imaginal) {
    this.real = real;
    this.imaginal = imaginal;
  }
  float real;
  float imaginal;
}

public static class Extensions {
  public static IEnumerable<IEnumerable<T>> Chunks<T>
    (this IEnumerable<T> list, int size) {
      while (list.Any ()) {
        yield return list.Take (size);
        list = list.Skip (size);
      }
    }
}

public class Fourier {

  public static fourierResult FullFourier (float[] input) {

    int N = input.Length;
    double real = 0;
    double imaginal = 0;
    int cost = 0;
    double notch = 2.0f * Math.PI / N;

    fourierResult result = new fourierResult ();

    Functions.SquareWalk (input,
      y => {
        real = 0;
        imaginal = 0;
      },
      (x, y) => {
        double r = notch * x * y;
        real += input[x] * Math.Cos (r);
        imaginal -= input[x] * Math.Sin (r);
        cost++;
      },
      y => {
        result.Add (new fourierValue (
          (float) real,
          (float) imaginal
        ));
      }
    );
    //  console.error(`TOTAL COST = ${cost}`);
    //  console.error(`TIME = ${Date.now() - markStart}msec`);
    return result;
  }

  public static IEnumerable<fourierResult> FastFourier (float[] input, int sample_count = 256) {
    for (int i = 0; i < input.Length; i += sample_count) {

      yield return FullFourier (input.Skip (i).Take (sample_count).ToArray ());
    }
    yield break;

  }

}