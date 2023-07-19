Shader "Custom/Zero"
{
 Properties {
  _MainTex2 ("Base (RGB)", 2D) = "white" {}
  _MainTexH ("HDR (HDR)", 2D) = "hdr" {}
  _bwBlend ("Black & White blend", Range (0, 1)) = 0.5
  _CameraPos ("Camera position" , Vector ) = ( 1, 1, 1, 1 )
  _CameraTar ("Camera target"   , Vector ) = ( 1, 1, 1, 1 ) 
  _Sphere1 ("Sphere1 pos"   , Vector ) = ( 1, 1, 1, 1 ) 
  _Sphere2 ("Sphere2 pos"   , Vector ) = ( 1, 1, 1, 1 ) 
  _Sphere3 ("Sphere3 pos"   , Vector ) = ( 1, 1, 1, 1 ) 
  _aspect ("Aspect Ratio", Float) = 1.0
  _Seed ("Seed", Int) = 1
  _fFOV ("Field of View", Range(1,179)) = 60.0
  _fShadePower ("Shade Power", Range(0.1,20.0)) = 2.0
  _Sphere1R("Sphere1 r", Range(0.000,20.0)) = 0.5	 
  _Sphere2R("Sphere2 r", Range(0.000,20.0)) = 0.5
  _Sphere3R("Sphere3 r", Range(0.000,20.0)) = 0.5
 }
 
 SubShader {
 Pass {
 CGPROGRAM
 #pragma vertex vert_img
 #pragma fragment frag
  
 #include "UnityCG.cginc"
 
 uniform sampler2D _MainTex2;
 uniform sampler2D _MainTexH;
 uniform float _bwBlend;

	struct Ray {
		float3 origin;
		float3 direction;
	};

	struct Light {
		float3 color;
		float3 direction;
	};

	struct Material {
		float3 color;
		float diffuse;
		float specular;
	};

	struct Intersect {
		float len;
		float3 normal;
		int nMaterial;
        float4 res;
	};

	struct Sphere {
		float radius;
		float3 position;
		int nMaterial;
	};

	struct Plane {
		float3 normal;
		float3 d;
		int nMaterial;
	};

	float4 _CameraPos;
	float4 _CameraTar;
	float4 _Sphere1;
	float4 _Sphere2;
	float4 _Sphere3;
	float _Sphere1R;
	float _Sphere2R;
	float _Sphere3R;

	float _aspect;
	float _fFOV;
    float _fShadePower;
	int _Seed;
	
	const float epsilon = 1e-3;

	static Sphere sphere;
	static Sphere sphere2;
	static Sphere sphere3;
	static Sphere sphereHDR;
	static Plane plane;
	static Intersect miss;
	
	Intersect intersect(Ray ray, Plane plane) 
	{
		float len = (-dot(ray.origin, plane.normal)+plane.d) / dot(ray.direction, plane.normal);
		
		Intersect res;
		if (len < 0.0) return miss;
		//{
			//res.len = -1.0;
			//return res;
		//}

		res.len = len;
		res.normal = plane.normal;
		res.nMaterial = plane.nMaterial;
		return res;
	}

	Intersect intersect(Ray ray, Sphere sphere)
	{
		// Check for a Negative Square Root
		float3 oc = sphere.position - ray.origin;
		float l = dot(ray.direction, oc);
		float det = pow(l, 2.0) - dot(oc, oc) + pow(sphere.radius, 2.0);
		if (det < 0.0) return miss;
		
		// Find the Closer of Two Solutions
				 float len = l - sqrt(det);
		if (len < 0.0) len = l + sqrt(det);
		if (len < 0.0) return miss;

		
		Intersect r;
		r.len = len;
		r.normal = (ray.origin + len*ray.direction - sphere.position) / sphere.radius;
		r.nMaterial = sphere.nMaterial;

		return r;
	}

	static const float PI = 3.14159265f;

	float3 GetHDRSample(float3 fDir)
	{		
		float2 uv = float2(0.5 + 0.5*atan2(-fDir.z,fDir.x)/PI, acos(-fDir.y)/PI);	
		return tex2D(_MainTexH, uv);//*(1.0+sin(_Time.y));
	}


	float3 GetHDRSampleRay(Ray ray)
	{
		//float3 origin;
		//float3 direction;
		//float radius;
		//float3 position;


		Sphere sphere;
		sphere.position = float3(0, 0, 0);
		sphere.radius   = 300.0;
		Intersect si = intersect(ray, sphere);

		float3 po = ray.origin + si.len * ray.direction;
		po = normalize(po);

		return GetHDRSample(po);
	}



	float3 vectYY(float yaw,float pitch)
	{
		yaw   =   (yaw/180.0)*PI;
		pitch = (pitch/180.0)*PI;

		float x = cos(yaw)*cos(pitch);
		float y = sin(yaw)*cos(pitch);
		float z = sin(pitch);

		return float3(x,y,z);
	}

	float rand(float3 co)
	{
	   return abs(frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453));
	}

	float2 seed;

	float2 rand2n() 
	{
		seed+=float2(-1,1);
		// implementation based on: lumina.sourceforge.net/Tutorials/Noise.html
		return float2(frac(sin(dot(seed.xy ,float2(12.9898,78.233))) * 43758.5453),
					  frac(cos(dot(seed.xy ,float2(4.898,7.23))) * 23421.631));
	}

	float3 LightPos()
	{
		      float2 xxx = rand2n();
        const float  a = xxx.x*360.0;// rand(v)*360.0;//+1.0);
        const float  b = xxx.y*90.0;//ng.GetRandomFloat(0,90.0);//rand(v)*90.0;//+1.0);
		return vectYY(a,b);
	}


    Intersect Trace(Ray ray)
	{
		//float4 result = float4(1,1,0,1);
		Intersect interR; 
		interR.len=-1.0;
		Intersect inter; 
		inter.len=-1.0;
        const float4 sphC = float4(0.5,0.5,0.5,1);
		float fMin = 10000.0;

		if (sphere.radius > 0.001)
		{
			inter = intersect(ray, sphere);
			if (inter.len != -1.0)
			{
				fMin = inter.len;
				inter.res.rgb = sphC;
				interR = inter;
			}
		}

		if (sphere2.radius > 0.001)
		{
			inter = intersect(ray, sphere2);
			if (inter.len != -1.0 && inter.len < fMin)
			{
				fMin = inter.len;
				inter.res.rgb = sphC;
				interR = inter;
			}
		}

		if (sphere3.radius > 0.001)
		{
			inter = intersect(ray, sphere3);
			if (inter.len != -1.0 && inter.len < fMin)
			{
				fMin = inter.len;
				inter.res.rgb = sphC;
				interR = inter;
			}
		}

		inter = intersect(ray,plane);
		if (inter.len!=-1.0 && inter.len<fMin)
		{
			fMin = inter.len;
			float3 pos = ray.origin + fMin*ray.direction;
			float ss = 0.1;
			fixed4 sam = tex2D(_MainTex2, float2(pos.x*ss,pos.z*ss));//dot(float3(0,1,0),inter.normal);		
			inter.res.rgb = sam.rgb;
			interR = inter;	
			//result.y = pos.x*ss;
		}

		inter = intersect(ray, sphereHDR);
		if (inter.len != -1.0 && inter.len<fMin)
		{
			fMin = 10000.0;
			inter.res.rgb = GetHDRSampleRay(ray);
			inter.len = -1.0;
			interR = inter;
		}


		//hdr background
		if (fMin==10000.0)//inter.len==-1.0)// && inter.len<fMin)
		{
			//half4 skyData = UNITY_SAMPLE_TEXCUBE(_MainTexH, ray.direction);
			//result.rgb = tex2D(_MainTexH, ray.direction);
			//inter.res.rgb = GetHDRSampleRay(ray); //float3(1,0,0);
			//inter.len = -1.0;
			//interR = inter;	
		}
		else
		{
			
			//interR.res.rgb = float4(1,1,1,1);//pow(dot(-ray.direction,interR.normal),3);			
			//interR.res.rgb = float4(0,0,0,1);//pow(dot(float3(0,1,0),interR.normal),3);			
		}

		return interR;
	}

	float4 Shade(Ray rayS,Intersect i)
	{
        const int nRays = 64;
		float3 fRes = float3(0.0,0.0,0.0);

		rayS.origin += (i.len-0.0001)*rayS.direction;

		for (int id=0;id<nRays;id++)
		{			
			rayS.direction = LightPos();
			float3 cc = GetHDRSampleRay(rayS);

			Intersect i1 = Trace(rayS);
			if (i1.len==-1.0) fRes+=cc*dot(rayS.direction,i.normal);
		}

		return float4(_fShadePower*fRes/float(nRays),1);
	}
	
	Intersect TraceAndShade(Ray ray)
	{
		Intersect i = Trace(ray);
		if (i.len!=-1) i.res *= Shade(ray,i);
        return i;
	}

	float rSchlick2Refl(float3 normal, float3 incident)
	{	
		float cosX = abs(dot(normal, incident));
		const float x = 1.0 - cosX;
		return 1.0-x*x*x*x*x;
	}

    float GetReflection(int nMaterial)
	{
		return nMaterial==0 ? 0.9 : 0.2;
	}
    
    float4 TraceR(Ray ray)
	{
        Intersect i1 = TraceAndShade(ray);
        float fRef = GetReflection(i1.nMaterial);
		float4 result = i1.res;
        //const float fRefl = 0.8;

		if (i1.len!=-1.0)
		{
			ray.origin += (i1.len-0.0001)*ray.direction;
			ray.direction = reflect(ray.direction,i1.normal);
			float s = rSchlick2Refl(i1.normal,ray.direction);
			
			Intersect i2 = TraceAndShade(ray);
			fRef*= GetReflection(i2.nMaterial);
            result+= fRef*s*i2.res;

			if (i2.len!=-1.0)
			{
				ray.origin += (i2.len-0.0001)*ray.direction;
				ray.direction = reflect(ray.direction, i2.normal);				
				float s = rSchlick2Refl(i2.normal,ray.direction);
				
				Intersect i3 = TraceAndShade(ray);
				fRef*= GetReflection(i3.nMaterial);
				result+= fRef*s*i3.res;
			}
		}
	
		return result;
	}

 
	float4 frag(v2f_img i) : COLOR 
    {
		float4 c = tex2D(_MainTex2, i.uv);
		seed = i.uv.xy *_Seed;

 
		float an = 0.0;//_Time.y;//0.5;
	
		float2 p = i.uv;
		p-=float2(0.5,0.5);
		p.x*=-1.0;

		//-----------------------------------------------------
		// camera
		//-----------------------------------------------------
		float3 ro = float3(_CameraPos.x,_CameraPos.y,_CameraPos.z);
		float3 ta = float3(_CameraTar.x,_CameraTar.y,_CameraTar.z);
		float3 ww = normalize( ta - ro );
		float3 uu = normalize( cross(ww,float3(0.0,1.0,0.0) ) );
		float3 vv = normalize( cross(uu,ww));
		const float Deg2Rad = UNITY_PI/180.0;

		float s = 2.0;//UNITY_PI;//2.0;//0.25*UNITY_PI;//UNITY_PI;//2.0*UNITY_PI;
		//float3 Xinc = (s*tan(_fFOV*Deg2Rad*0.5))*uu;
		//float3 Yinc = (s*tan(_fFOV*Deg2Rad*0.5))*vv;
		//float3 rd   = normalize( p.x*Xinc + p.y*Yinc + 1.0*ww );
		float3 Xinc = (s*tan(_fFOV*Deg2Rad*0.5))*uu*_aspect;
		float3 Yinc = (s*tan(_fFOV*Deg2Rad*0.5))*vv;
		//float ff = s*tan(_fFOV*Deg2Rad*0.5);
		float3 rd = normalize( p.x*Xinc + p.y*Yinc + 1.0*ww );

		miss.len = -1.0;

		sphere.radius = _Sphere1R;
		sphere.position = _Sphere1;//float3(0,0,0);
		sphere.nMaterial = 0;

		sphere2.radius = _Sphere2R;
		sphere2.position = _Sphere2;//float3(1,0,0);
		sphere2.nMaterial = 0;

		sphere3.radius = _Sphere3R;
		sphere3.position = _Sphere3;//1float3(0,0,-1);
		sphere3.nMaterial = 0;

		sphereHDR.radius = 300.;
		sphereHDR.position = float3(0, 0, 0);
		sphereHDR.nMaterial = 0;



		plane.normal = float3(0,1,0);
		plane.d = 0.0;//0.5*sin(an);
		plane.nMaterial = 1;

		Ray ray;
		ray.origin = ro;
		ray.direction = rd;

		return TraceR(ray);
 }
 ENDCG
 }
 }
}
