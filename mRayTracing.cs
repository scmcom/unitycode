//ORIGINAL CODE: Ray Tracing & Photon Mapping : Grant Schindler, 2007 : http://www.cc.gatech.edu/~phlosoft/photon/
// ----------------------------------------------------------------------
// converted to Unity3D c# v1.0 24.01.2012 - mgear - http://unitycoder.com/blog
// missing: interface, user interactions and original speed..Oo


using UnityEngine;
using System.Collections;

public class mRayTracing : MonoBehaviour 
{
	
	private Texture2D texture;
	
	private float time;
	
	int szImg = 512;                  //Image Size
	
	Color[] colarray;
	
	int nrTypes = 2;                  //2 Object Types (Sphere = 0, Plane = 1)
	int[] nrObjects = {2,5};          //2 Spheres, 5 Planes
	float gAmbient = 0.1f;             //Ambient Lighting
	float[] gOrigin = {0.0f,0.0f,0.0f};  //World Origin for Convenient Re-Use Below (Constant)
	float[] Light = {0.0f,1.2f,3.75f};   //Point Light-Source Position
	
	int prevMouseX = -9999, prevMouseY = -9999, sphereIndex = -1;
	float s = 130.0f; //Arbitary Constant Through Experimentation
	bool mouseDragging = false, gettime=false;
	
	float[][] spheres = new float[][] 
	{
		new float[] {1.0f,0.0f,4.0f,0.5f},
		new float[] {-0.6f,-1.0f,4.5f,0.5f}
	};

	float[][] planes = new float[][] 
	{
		new float[] {0, 1.5f},
		new float[] {1, -1.5f},
		new float[] {0, -1.5f},
		new float[] {1, 1.5f},
		new float[] {2,5.0f}
	};
	
	// ----- Photon Mapping -----
	int nrPhotons = 1000;             //Number of Photons Emitted
	int nrBounces = 3;                //Number of Times Each Photon Bounces
	bool lightPhotons = true;      //Enable Photon Lighting?
	float sqRadius = 0.7f;             //Photon Integration Area (Squared for Efficiency)
	float exposure = 50.0f;            //Number of Photons Integrated at Brightest Pixel
	
	int[][] numPhotons = new int[][] 
	{
		new int[] {0,0},
		new int[] {0,0,0,0,0}
	};

	float[][][][][] photons = new float[2][][][][]; //Allocated Memory for Per-Object Photon Info


	// ----- Raytracing Globals -----
	bool gIntersect = false;       //For Latest Raytracing Call... Was Anything Intersected by the Ray?
	int gType;                        //... Type of the Intersected Object (Sphere or Plane)
	int gIndex;                       //... Index of the Intersected Object (Which Sphere/Plane Was It?)
	float gSqDist, gDist = -1.0f;      //... Distance from Ray Origin to Intersection
	float[] gPoint = {0.0f, 0.0f, 0.0f}; //... Point At Which the Ray Intersected the Object

	float[] cache;
	
	// Use this for initialization
	void Start () 
	{
		
		
		colarray = new Color[szImg*szImg];
		
		time = Time.time;
		
		texture = new Texture2D(szImg, szImg);
		GetComponent<Renderer>().material.mainTexture = texture;

		for(int i1 = 0; i1 < photons.Length; ++i1)
		{photons[i1] = new float[5][][][];for(int i2 = 0; i2 < photons[i1].Length; ++i2) 
			{photons[i1][i2] = new float[5000][][];for(int i3 = 0; i3 <photons[i1][i2].Length; ++i3) 
				{photons[i1][i2][i3] = new float[3][];	for(int i4 = 0; i4 < photons[i1][i2][i3].Length; ++i4) 
					{photons[i1][i2][i3][i4] = new float[3];}}}}
	
	    // Simple lookup function for the test.
//        cache = {};
//		cache = new float[30];
//        for(int i = 0; i < 30; i++)
//        {
//            cache[i] = Mathf.Sqrt(i);
//        }
		

	  emitPhotons();
	  resetRender(); 
	  //drawInterface();
	  cleartexture();

		
	}
	
	// Update is called once per frame
	void Update () 
	{
		draw();
		texture.SetPixels (colarray);
		texture.Apply();
	}
	
	
	
	
	void cleartexture()
	{
	  // clear texture	
	  for ( int x = 0; x < szImg; x++ ){
		for ( int y = 0; y < szImg; y++ ){
			texture.SetPixel(x, y, new Color(0,0,0.1f,1));
		}
	}
	texture.Apply();
	}
	
	void raySphere(int idx, float[] r, float[] o) //Ray-Sphere Intersection: r=Ray Direction, o=Ray Origin
	{ 
	  float[] s = sub3(spheres[idx],o);  //s=Sphere Center Translated into Coordinate Frame of Ray Origin
	  float radius = spheres[idx][3];    //radius=Sphere Radius
	  
	  //Intersection of Sphere and Line     =       Quadratic Function of Distance
	  float A = dot3(r,r);                       // Remember This From High School? :
	  float B = -2.0f * dot3(s,r);                //    A x^2 +     B x +               C  = 0
	  float C = dot3(s,s) - sq(radius);          // (r'r)x^2 - (2s'r)x + (s's - radius^2) = 0
	  float D = B*B - 4*A*C;                     // Precompute Discriminant
	  
	  if (D > 0.0){                              //Solution Exists only if sqrt(D) is Real (not Imaginary)
		float sign = (C < -0.00001) ? 1 : -1;    //Ray Originates Inside Sphere If C < 0
		float lDist = (-B + sign*sqrt(D))/(2*A); //Solve Quadratic Equation for Distance to Intersection
		checkDistance(lDist,0,idx);}             //Is This Closest Intersection So Far?
	}
	
	
	// own functions, 	Squares a number (multiplies a number by itself)
	float sq(float v)
	{
		return v*v;
	}
	
	float sqrt(float v) 
	{
		return Mathf.Sqrt(v); 

	}

	float random(float low,float high)
	{
		return Random.Range(low,high);
	}
	
	float min(float v1,float v2)
	{
		return Mathf.Min(v1,v2);
	}

	float max(float v1,float v2)
	{
		return Mathf.Max(v1,v2);
	}

	float abs(float v)
	{
		return Mathf.Abs(v);
	}


	float pow(float num,float exponent)
	{
		return Mathf.Pow(num,exponent);
	}
	
	// ----- Scene Description -----

	//---------------------------------------------------------------------------------------
	//Ray-Geometry Intersections  -----------------------------------------------------------
	//---------------------------------------------------------------------------------------



	void rayPlane(int idx, float[] r, float[] o){ //Ray-Plane Intersection
	  int axis = (int) planes[idx][0];            //Determine Orientation of Axis-Aligned Plane
	  if (r[axis] != 0.0){                        //Parallel Ray -> No Intersection
		float lDist = (planes[idx][1] - o[axis]) / r[axis]; //Solve Linear Equation (rx = p-o)
		checkDistance(lDist,1,idx);}
	}

	void rayObject(int type, int idx, float[] r, float[] o){
	  if (type == 0) raySphere(idx,r,o); else rayPlane(idx,r,o);
	}

	void checkDistance(float lDist, int p, int i){
	  if (lDist < gDist && lDist > 0.0){ //Closest Intersection So Far in Forward Direction of Ray?
		gType = p; gIndex = i; gDist = lDist; gIntersect = true;} //Save Intersection in Global State
	}
	
	//---------------------------------------------------------------------------------------
	//Vector Operations ---------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	float[] normalize3(float[] v){        //Normalize 3-Vector
		Vector3 tmp = new Vector3(v[0],v[1],v[2]).normalized;
		v[0] = tmp.x;
		v[1] = tmp.y;
		v[2] = tmp.z;
		return v;
		
	}

	float[] sub3(float[] a, float[] b){   //Subtract 3-Vectors
	  float[] result = {a[0] - b[0], a[1] - b[1], a[2] - b[2]};
	  return result;
	}

	float[] add3(float[] a, float[] b){   //Add 3-Vectors
	  float[] result = {a[0] + b[0], a[1] + b[1], a[2] + b[2]};
	  return result;
	}

	float[] mul3c(float[] a, float c){    //Multiply 3-Vector with Scalar
	  float[] result = {c*a[0], c*a[1], c*a[2]};
	  return result;
	}

	float dot3(float[] a, float[] b){     //Dot Product 3-Vectors
	  return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
	}

	float[] rand3(float s){               //Random 3-Vector
	  float[] rand = {random(-s,s),random(-s,s),random(-s,s)};
	  return rand;
	}

	bool gatedSqDist3(float[] a, float[] b, float sqradius){ //Gated Squared Distance
	  float c = a[0] - b[0];          //Efficient When Determining if Thousands of Points
	  float d = c*c;                  //Are Within a Radius of a Point (and Most Are Not!)
	  if (d > sqradius) return false; //Gate 1 - If this dimension alone is larger than
	  c = a[1] - b[1];                //         the search radius, no need to continue
	  d += c*c;
	  if (d > sqradius) return false; //Gate 2
	  c = a[2] - b[2];
	  d += c*c;
	  if (d > sqradius) return false; //Gate 3
	  gSqDist = d;      return true ; //Store Squared Distance Itself in Global State
	}
	
	
	//---------------------------------------------------------------------------------------
	// Lighting -----------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	float lightDiffuse(float[] N, float[] P){  //Diffuse Lighting at Point P with Surface Normal N
	  float[] L = normalize3( sub3(Light,P) ); //Light Vector (Point to Light)
	  return dot3(N,L);                        //Dot Product = cos (Light-to-Surface-Normal Angle)
	}

	float[] sphereNormal(int idx, float[] P){
	  return normalize3(sub3(P,spheres[idx])); //Surface Normal (Center to Point)
	}

	float[] planeNormal(int idx, float[] P, float[] O){
	  int axis = (int) planes[idx][0];
	  float [] N = {0.0f,0.0f,0.0f};
	  N[axis] = O[axis] - planes[idx][1];      //Vector From Surface to Light
	  return normalize3(N);
	}

	float[] surfaceNormal(int type, int index, float[] P, float[] Inside){
	  if (type == 0) {return sphereNormal(index,P);}
	  else           {return planeNormal(index,P,Inside);}
	}

	float lightObject(int type, int idx, float[] P, float lightAmbient){
	  float i = lightDiffuse( surfaceNormal(type, idx, P, Light) , P );
	  return min(1.0f, max(i, lightAmbient));   //Add in Ambient Light by Constraining Min Value
	}
	
	

	//---------------------------------------------------------------------------------------
	// Raytracing ---------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	void raytrace(float[] ray, float[] origin)
	{
	  gIntersect = false; //No Intersections Along This Ray Yet
	  gDist = 5.9f;   //99999.9f Maximum Distance to Any Object
	  
	  for (int t = 0; t < nrTypes; t++)
		for (int i = 0; i < nrObjects[t]; i++)
		  rayObject(t,i,ray,origin);
	}

	float[] computePixelColor(float x, float y)
	{
	  float[] rgb = {0.0f,0.0f,0.0f};
	  float[] ray = {  x/szImg - 0.5f ,       //Convert Pixels to Image Plane Coordinates
					 -(y/szImg - 0.5f), 1.0f}; //Focal Length = 1.0
					 
	  raytrace(ray, gOrigin);                //Raytrace!!! - Intersected Objects are Stored in Global State

	  if (gIntersect){                       //Intersection                    
		gPoint = mul3c(ray,gDist);           //3D Point of Intersection
			
		if (gType == 0 && gIndex == 1){      //Mirror Surface on This Specific Object
		  ray = reflect(ray,gOrigin);        //Reflect Ray Off the Surface
		  raytrace(ray, gPoint);             //Follow the Reflected Ray
		  if (gIntersect){ gPoint = add3( mul3c(ray,gDist), gPoint); }} //3D Point of Intersection
			
		if (lightPhotons){                   //Lighting via Photon Mapping
		  rgb = gatherPhotons(gPoint,gType,gIndex);}
		else{                                //Lighting via Standard Illumination Model (Diffuse + Ambient)
		  int tType = gType, tIndex = gIndex;//Remember Intersected Object
		  float i = gAmbient;                //If in Shadow, Use Ambient Color of Original Object
		  raytrace( sub3(gPoint,Light) , Light);  //Raytrace from Light to Object
		  if (tType == gType && tIndex == gIndex) //Ray from Light->Object Hits Object First?
			i = lightObject(gType, gIndex, gPoint, gAmbient); //Not In Shadow - Compute Lighting
		  rgb[0]=i; rgb[1]=i; rgb[2]=i;
		  rgb = getColor(rgb,tType,tIndex);}
	  }
	  return rgb;
	}

	float[] reflect(float[] ray, float[] fromPoint){                //Reflect Ray
	  float[] N = surfaceNormal(gType, gIndex, gPoint, fromPoint);  //Surface Normal
	  return normalize3(sub3(ray, mul3c(N,(2 * dot3(ray,N)))));     //Approximation to Reflection
	}

		
	//---------------------------------------------------------------------------------------
	//Photon Mapping ------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	float[] gatherPhotons(float[] p, int type, int id){
	  float[] energy = {0.0f,0.0f,0.0f};  
	  float[] N = surfaceNormal(type, id, p, gOrigin);                   //Surface Normal at Current Point
	  for (int i = 0; i < numPhotons[type][id]; i++){                    //Photons Which Hit Current Object
		  
		  // [,,,] is not same as [][][] for the function gatedSqDist3...
		if (gatedSqDist3(p,photons[type][id][i][0],sqRadius)){           //Is Photon Close to Point?
		//if (gatedSqDist3(p,photons[type,id,i,0],sqRadius)){           //Is Photon Close to Point?
		  float weight = max(0.0f, -dot3(N, photons[type][id][i][1] ));   //Single Photon Diffuse Lighting
//		  float weight = max(0.0f, -dot3(N, photons[type,id,i,1] ));   //Single Photon Diffuse Lighting
		  weight *= (1.0f - sqrt(gSqDist)) / exposure;                    //Weight by Photon-Point Distance
		  energy = add3(energy, mul3c(photons[type][id][i][2], weight)); //Add Photon's Energy to Total
//		  energy = add3(energy, mul3c(photons[type,id,i,2], weight)); //Add Photon's Energy to Total
	   }} 
	  return energy;
	}

	void emitPhotons(){
	  //randomSeed(0);                               //Ensure Same Photons Each Time
	  Random.seed =0;
	  for (int t = 0; t < nrTypes; t++)            //Initialize Photon Count to Zero for Each Object
		for (int i = 0; i < nrObjects[t]; i++)
		  numPhotons[t][i] = 0; 

	  for (int i = 0; i < (view3D ? nrPhotons * 3.0f : nrPhotons); i++){ //Draw 3x Photons For Usability
//	  for (int i = 0; i < nrPhotons * 3.0f; i++){ //Draw 3x Photons For Usability
		int bounces = 1;
		float[] rgb = {1.0f,1.0f,1.0f};               //Initial Photon Color is White
		float[] ray = normalize3( rand3(1.0f) );    //Randomize Direction of Photon Emission
		float[] prevPoint = Light;                 //Emit From Point Light Source
		
		//Spread Out Light Source, But Don't Allow Photons Outside Room/Inside Sphere
		while (prevPoint[1] >= Light[1]){ prevPoint = add3(Light, mul3c(normalize3(rand3(1.0f)), 0.75f));}
		if (abs(prevPoint[0]) > 1.5 || abs(prevPoint[1]) > 1.2f ||
			gatedSqDist3(prevPoint,spheres[0],spheres[0][3]*spheres[0][3])) bounces = nrBounces+1;
		
		raytrace(ray, prevPoint);                          //Trace the Photon's Path
		
		while (gIntersect && bounces <= nrBounces){        //Intersection With New Object
			gPoint = add3( mul3c(ray,gDist), prevPoint);   //3D Point of Intersection
			rgb = mul3c (getColor(rgb,gType,gIndex), 1.0f/sqrt(bounces));
			storePhoton(gType, gIndex, gPoint, ray, rgb);  //Store Photon Info 
			drawPhoton(rgb, gPoint);                       //Draw Photon
			shadowPhoton(ray);                             //Shadow Photon
			ray = reflect(ray,prevPoint);                  //Bounce the Photon
			raytrace(ray, gPoint);                         //Trace It to Next Location
			prevPoint = gPoint;
			bounces++;}
	  }
	}


	void storePhoton(int type, int id, float[] location, float[] direction, float[] energy)
	{
	  photons[type][id][numPhotons[type][id]][0] = location;  //Location
	  photons[type][id][numPhotons[type][id]][1] = direction; //Direction
	  photons[type][id][numPhotons[type][id]][2] = energy;    //Attenuated Energy (Color)
	  numPhotons[type][id]++;
	}

	void shadowPhoton(float[] ray){                               //Shadow Photons
	  float[] shadow = {-0.25f,-0.25f,-0.25f};
	  float[] tPoint = gPoint; 
	  int tType = gType, tIndex = gIndex;                         //Save State
	  float[] bumpedPoint = add3(gPoint,mul3c(ray,0.00001f));      //Start Just Beyond Last Intersection
	  raytrace(ray, bumpedPoint);                                 //Trace to Next Intersection (In Shadow)
	  float[] shadowPoint = add3( mul3c(ray,gDist), bumpedPoint); //3D Point
	  storePhoton(gType, gIndex, shadowPoint, ray, shadow);
	  gPoint = tPoint; gType = tType; gIndex = tIndex;            //Restore State
	}

	float[] filterColor(float[] rgbIn, float r, float g, float b){ //e.g. White Light Hits Red Wall
	  float[] rgbOut = {r,g,b};
	  for (int c=0; c<3; c++) rgbOut[c] = min(rgbOut[c],rgbIn[c]); //Absorb Some Wavelengths (R,G,B)
	  return rgbOut;
	}

	float[] getColor(float[] rgbIn, int type, int index){ //Specifies Material Color of Each Object
	  if      (type == 1 && index == 0) { return filterColor(rgbIn, 0.0f, 1.0f, 0.0f);}
	  else if (type == 1 && index == 2) { return filterColor(rgbIn, 1.0f, 0.0f, 0.0f);}
	  else                              { return filterColor(rgbIn, 1.0f, 1.0f, 1.0f);}
	}

		//---------------------------------------------------------------------------------------
	// User Interaction and Display ---------------------------------------------------------
	//---------------------------------------------------------------------------------------
	bool empty = true , view3D = false; //Stop Drawing, Switch Views
//	PFont font; PImage img1, img2, img3;  //Fonts, Images
	int pRow, pCol, pIteration, pMax;     //Pixel Rendering Order
	
	bool odd(int x) {return x % 2 != 0;}
	
//	void setup(){
//	  size(szImg,szImg + 48, JAVA2D);
//	  frameRate(9999);
//	  font = loadFont("Helvetica-Bold-12.vlw");
//	}

	void draw()
	{  
	  if (view3D)
	  {
			if (empty)
				{
				emitPhotons(); 
				empty = false; //frameRate(10);}} //Emit & Draw Photons
			}
		}else{
			if (empty) render(); //else frameRate(10);} //Only Draw if Image Not Fully Rendered
			
			if (!empty) 
			{
				if (!gettime)
				{
//					texture.Apply();
					texture.SetPixels (colarray);
					texture.Apply();

					
					print (Time.time - time);
					gettime = true;

					

				}
			}
			
		}
	}

	void drawInterface() {
//	  stroke(221,221,204); fill(221,221,204); rect(0,szImg,szImg,48); //Fill Background with Page Color 
//	  img1=loadImage("1_32.png"); img2=loadImage("2_32.png"); img3=loadImage("3_32.png"); //Load Images
//	  
//	  textFont(font); //Display Text
//	  if (!view3D) {fill(0); img3.filter(GRAY);} else fill(160);
//	  text("Ray Tracing", 64, szImg + 28);
//	  if (lightPhotons || view3D) {fill(0); img1.filter(GRAY);} else fill(160);
//	  text("Photon Mapping", 368, szImg + 28);
//	  if (!lightPhotons || view3D) img2.filter(GRAY);
	  
//	  stroke(0); fill(255);  //Draw Buttons with Icons
//	  rect(198,519,33,33); image(img1,199,520);
//	  rect(240,519,33,33); image(img2,241,520);
//	  rect(282,519,33,33); image(img3,283,520);
	}


	
	void render(){ //Render Several Lines of Pixels at Once Before Drawing
	  int x,y,iterations = 0;
	  float[] rgb = {0.0f,0.0f,0.0f};
	  
	  while (iterations < (mouseDragging ? 1024 : max(pMax, 256) ))
	  {
			//Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
			if (pCol >= pMax) 
				{
					pRow++; pCol = 0; 
					if (pRow >= pMax) 
					{
						pIteration++;
						pRow = 0;
						pMax = (int) pow(2,pIteration);
					}
				}
			bool pNeedsDrawing = (pIteration == 1 || odd(pRow) || (!odd(pRow) && odd(pCol)));
			x = pCol * (szImg/pMax); y = pRow * (szImg/pMax);
			pCol++;
				
			if (pNeedsDrawing){
			  iterations++;
//			  rgb = mul3c( computePixelColor(x,y), 255.0f);               //All the Magic Happens in Here!
			  rgb = computePixelColor(x,y);               //All the Magic Happens in Here!
			   
//			  texture.SetPixel(x, y, new Color(rgb[0]/255,rgb[1]/255,rgb[2]/255,1));
			  //texture.SetPixel(x, y, new Color(rgb[0],rgb[1],rgb[2],1));
			 colarray[y*szImg+x] =  new Color(rgb[0],rgb[1],rgb[2],1);
				
	//		  stroke(rgb[0],rgb[1],rgb[2]); fill(rgb[0],rgb[1],rgb[2]);  //Stroke & Fill
	//		  rect(x,y,(szImg/pMax)-1,(szImg/pMax)-1);}                  //Draw the Possibly Enlarged Pixel
			}
		}
	  if (pRow == szImg-1) {empty = false;}
	}

	void resetRender(){ //Reset Rendering Variables
	  pRow=0; pCol=0; pIteration=1; pMax=2; //frameRate(9999);
	  empty=true; if (lightPhotons && !view3D) emitPhotons();}

	void drawPhoton(float[] rgb, float[] p)
	{           //Photon Visualization
	  if (view3D && p[2] > 0.0){                       //Only Draw if In Front of Camera
		int x = (szImg/2) + (int)(szImg *  p[0]/p[2]); //Project 3D Points into Scene
		int y = (szImg/2) + (int)(szImg * -p[1]/p[2]); //Don't Draw Outside Image
		if (y <= szImg) 
		{
//			stroke(255.0f*rgb[0],255.0f*rgb[1],255.0f*rgb[2]);
//			point(x,y);
		}
		}
	}

	
	
	//---------------------------------------------------------------------------------------
	//Mouse and Keyboard Interaction --------------------------------------------------------
	//---------------------------------------------------------------------------------------
	
	void mouseReleased() {prevMouseX = -9999; prevMouseY = -9999; mouseDragging = false;}
//	void keyPressed() {switchToMode(key,9999);}
/*	   
	void mousePressed(){
	  sphereIndex = 2; //Click Spheres
	  float[] mouse3 = {(mouseX - szImg/2)/s, -(mouseY - szImg/2)/s, 0.5*(spheres[0][2] + spheres[1][2])};
	  if (gatedSqDist3(mouse3,spheres[0],spheres[0][3])) sphereIndex = 0;
	  else if (gatedSqDist3(mouse3,spheres[1],spheres[1][3])) sphereIndex = 1;
	  if (mouseY > szImg) switchToMode('0', mouseX); //Click Buttons
	}

	void mouseDragged(){
	  if (prevMouseX > -9999 && sphereIndex > -1){
		if (sphereIndex < nrObjects[0]){ //Drag Sphere
		  spheres[sphereIndex][0] += (mouseX - prevMouseX)/s;
		  spheres[sphereIndex][1] -= (mouseY - prevMouseY)/s;}
		else{ //Drag Light
		  Light[0] += (mouseX - prevMouseX)/s; Light[0] = constrain(Light[0],-1.4,1.4);
		  Light[1] -= (mouseY - prevMouseY)/s; Light[1] = constrain(Light[1],-0.4,1.2);}
		resetRender();}
	 prevMouseX = mouseX; prevMouseY = mouseY; mouseDragging = true;
	}

	void switchToMode(char i, int x){ // Switch Between Raytracing, Photon Mapping Views
	  if      (i=='1' || x<230) {view3D = false; lightPhotons = false; resetRender(); drawInterface();}
	  else if (i=='2' || x<283) {view3D = false; lightPhotons = true;  resetRender(); drawInterface();}
	  else if (i=='3' || x<513) {view3D = true; resetRender(); drawInterface();}
	}
	*/	
	
}


