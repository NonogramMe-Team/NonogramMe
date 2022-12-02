using UnityEngine;
using System.Collections.Generic;      
using System.Runtime.InteropServices;    
using System;    
using System.Drawing;    
using System.IO;
using System.Windows;
using OpenCvSharp;
using TMPro;
using SimpleFileBrowser;

public class Test : MonoBehaviour {

public GameObject loadO;
public UnityEngine.UI.Button load;

public TextMeshProUGUI success;

public UnityEngine.UI.Button quit;


Mat pic = new Mat();   
Mat modified = new Mat();
Mat boosted = new Mat();
Mat gaussianBlur = new Mat();
Mat sobelX = new Mat();
Mat sobelY = new Mat();
Mat sobelXY = new Mat();

public AudioSource select;

private string location;
   void Awake(){
      success.text="";
      loadO.SetActive(true);
      load.onClick.AddListener(loadFile);
      quit.onClick.AddListener(exitGame);
      location = Application.persistentDataPath;
      if(!(File.Exists(location+"/Index.txt"))){
         File.WriteAllText(location+"/Index.txt","0"+Environment.NewLine);         
      }
   }

   public string finalPath;

   public void exitGame(){
      select.Play();
      Application.Quit();
   }

   public void loadFile(){
      select.Play();
      FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png",".jpeg",".jfif",".tiff" ));
      StartCoroutine( ShowLoadDialogCoroutine() );
      //string fileType = NativeFilePicker.ConvertExtensionToFileType("jpg,jfif,jpeg,tiff,png");
      //NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
      //{
        // if (path != null){
          //  finalPath = path;
         //}
      //}, new string[] {fileType}); 
      //success.text="";
      //processPhoto(finalPath);
   }

    System.Collections.IEnumerator ShowLoadDialogCoroutine(){
      yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load File", "Load" );
         if(FileBrowser.Success){
            string finalPath = FileBrowserHelpers.GetFilename( FileBrowser.Result[0]);
            finalPath = Path.GetFullPath(finalPath);
            Debug.Log(finalPath);
            //string Find = "/";
            //string Replace = "//";
            //int Place = finalPath.IndexOf(Find);
            //success.text="";
            //finalPath = finalPath.Remove(Place, Find.Length).Insert(Place, Replace);
            processPhoto(finalPath);
         }
   }

    // Start is called before the first frame update
    void processPhoto(string filepath){
      
      //get image
       pic = Cv2.ImRead(filepath);

       int height = pic.Rows;
       int width = pic.Cols;


      HierarchyIndex[] heirarchy = new HierarchyIndex[width];
      OpenCvSharp.Point [][] contours = new OpenCvSharp.Point [(width/2)*(height/2)][];

      //resize to half size
      Cv2.Resize(pic, modified, new OpenCvSharp.Size(width/2,height/2));
      Cv2.Resize(pic, pic, new OpenCvSharp.Size(width/2,height/2));
      Cv2.Decolor(modified,modified,boosted);
      Cv2.EqualizeHist(modified,modified);

      modified.CopyTo(sobelX);
      modified.CopyTo(sobelY);
      modified.CopyTo(sobelXY);

      //edge detection
      Cv2.GaussianBlur(modified,gaussianBlur,new OpenCvSharp.Size(3,3),5.0);
      Cv2.Sobel(gaussianBlur,sobelX,sobelX.Depth(),1,0,5);
      Cv2.Sobel(gaussianBlur,sobelY,sobelY.Depth(),0,1,5);
      Cv2.Sobel(gaussianBlur,sobelXY,sobelXY.Depth(),1,1,5);

      Cv2.FindContours(sobelXY,out contours,out heirarchy,OpenCvSharp.RetrievalModes.Tree,OpenCvSharp.ContourApproximationModes.ApproxNone);
      Cv2.DrawContours(sobelXY,contours,-1, new OpenCvSharp.Scalar(0,255,0),1);

      var dict = new SortedDictionary<int,double>();

      //sort contours
      if(contours.Length>0){
         for(int i = 0; i < contours.Length; i++){
            double area = Cv2.ContourArea(contours[i]);
            OpenCvSharp.Rect rect = Cv2.BoundingRect(contours[i]);
            if(rect.Width>50&&rect.Height>30&&area>4000){
            dict.Add(i,area);
            }
         }
      }

      int left = 0;
      int right = 0;
      int top = 0;
      int bottom = 0;
      int counter = 0;

      //find crop dimensions
      foreach(var i in dict){
         int key = int.Parse(i.Key.ToString());
         OpenCvSharp.Rect rect = Cv2.BoundingRect(contours[key]);
            if(rect.Left<right-left){
               left=rect.Left;
            }
            if(rect.Right>right){
               right = rect.Right;
            }
            if(rect.Bottom>bottom){
               bottom = rect.Bottom;
            }
            if(rect.Top<top-bottom){
               top = rect.Top;
            }
            counter++;
         } 

      //adjust to square
      int side = 0;
      if(right-left<bottom-top){
         side = right -left;
      } else {
         side = bottom-top;
      }
      
      OpenCvSharp.Rect crop = new OpenCvSharp.Rect(left,top,side,side);

      //apply crop  
      Mat cropped = new Mat(sobelXY,crop);
      Mat croppedOg = new Mat(pic,crop);

      //divide into 100 square regions and get mean pixel of each region
      double [,] pixelMean = new double[10,10];
      height = cropped.Rows/10;
      width = cropped.Cols/10;

      for(int i = 0; i < cropped.Rows-height*i; i++){
         for(int j = 0; j < cropped.Cols-width*j; j++){

            OpenCvSharp.Rect region = new OpenCvSharp.Rect(i*height,j*width,height,width);
            Cv2.Rectangle(cropped,region,new OpenCvSharp.Scalar(255,0,0),1);
            Mat reg = new Mat(cropped,region);
            Scalar scal = Cv2.Mean(reg);

            //Mean returns a scalar object that represents four channels
            //Save to array as double
            int   v1 = (int) scal[0];
            int   v2 = (int) scal[1];
            int   v3 = (int) scal[2];
            int   v4 = (int) scal[3];
            pixelMean[i,j] = v1+v2+v3+v4/4;
            //Debug.Log("pixelMean: "+pixelMean[i,j]);

            }
         }
         
         double sum = 0;
         double largest = 0;
         double smallest = 50;

         //find average of all regions
         for(int i = 0; i < cropped.Rows-height*i; i++){
            for(int j = 0; j < cropped.Cols-width*j; j++){
               sum = sum + pixelMean[i,j];
               if(pixelMean[i,j]>largest){
                  largest = pixelMean[i,j];
               }
               if(pixelMean[i,j]<smallest){
                  smallest = pixelMean[i,j];
               }
            }
         }
         
         double mean = (sum-largest+smallest)/100;
         //Debug.Log("Threshold: "+mean);

         //Number the puzzle
         string output = "";
         var allLines = File.ReadAllLines(location+"/Index.txt");
         int puzzles = int.Parse(allLines[0]);
         puzzles++;
         allLines[0] = puzzles.ToString();
         File.WriteAllLines(location+"/Index.txt", allLines);
         output+=puzzles+"-";

         //Color by mean region value and copy to bool array
         for(int i = 0; i < cropped.Rows-height*i; i++){
            for(int j = 0; j < cropped.Cols-width*j; j++){
               
               OpenCvSharp.Rect region = new OpenCvSharp.Rect(i*height,j*width,height,width);
               Cv2.Rectangle(cropped,region,new OpenCvSharp.Scalar(255,0,0),1);
               Mat reg = new Mat(cropped,region);

               //Color and write to bit array to save
               if(pixelMean[i,j]>mean){
                  reg.SetTo(Scalar.White);
                  output+="0";
               } else {
                  reg.SetTo(Scalar.Black);
                  output+="1";
               }
               reg.CopyTo(cropped.SubMat(region));
               Cv2.Rectangle(cropped,region,new OpenCvSharp.Scalar(0,255,0),1);

            }     
         }

      //Show results
      //Cv2.ImShow("Puzzle",cropped);
      //Cv2.ImShow("Cropped",croppedOg);
      //Cv2.ImShow("Original",pic);
      success.text = filepath+" added!";      
      select.Play();


      cropped.SaveImage(location+"/"+puzzles+"B.jpg");
      croppedOg.SaveImage(location+"/"+puzzles+"A.jpg");
      File.AppendAllText(location+"/Index.txt", output + Environment.NewLine);
      }
}
