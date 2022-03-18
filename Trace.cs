using Unity.Mathematics;
using Unity.Physics;

public class Trace
{

	public static SphereDat Sphere ( float3 pos , float r )
		=> new SphereDat( pos , r );
	public static SphereDat Sphere ( (float x,float y, float z) pos , float r )
		=> new SphereDat( new float3{ x=pos.x , y=pos.y , z=pos.z } , r );

	// public static RayDat Ray ( float3 origin , float3 dir )
	// 	=> new RayDat( origin:origin , dir:dir );
	// public static RayDat Ray ( (float x,float y, float z) origin , (float x,float y, float z) dir )
	// 	=> new RayDat( origin:new float3{ x=origin.x , y=origin.y , z=origin.z } , dir:new float3{ x=dir.x , y=dir.y , z=dir.z } );
	// public static RayDat Ray ( UnityEngine.Ray ray )
	// 	=> new RayDat( origin:ray.origin , dir:ray.direction );

	public struct SphereDat : ITraceObject
	{
		public float3 position;
		public float radius;
		public SphereDat ( float3 pos , float r )
		{
			this.position = pos;
			this.radius = r;
		}
	}
	public struct RayDat : ITraceObject
	{
		public float3 origin;
		public float3 dir;
		public RayDat ( float3 origin , float3 dir )
		{
			this.origin = origin;
			this.dir = dir;
		}
	}

	public struct TraceDat<A,B> : ITrace<A,B>
		where A : ITraceObject
		where B : ITraceObject
	{
		public A a;
		A ITrace<A,B>.a => a;
		public B b;
		B ITrace<A,B>.b => b;
		public TraceDat ( A a , B b )
		{
			this.a = a;
			this.b = b;
		}
	}

	public interface ITraceObject {}
	public interface ITrace <A,B>
		where A : ITraceObject
		where B : ITraceObject
	{
		A a { get; }
		B b { get; }
	}

	// syntax example
	static void Test__1 ()
	{
		Trace
			.Sphere( (0,1,0) , 1 )
			.Ray( (-1,-1,-1) , (1,1,1) )
			.Cast( out bool didHit , out var raycastHit );
	}
	
	// TODO: https://forum.unity.com/threads/how-to-castray-on-spherecollider-struct-without-physics-spherecollider-create-allocation.984038/#post-6396377
	public static bool SphereRay
	(
		float3 spherePos ,
		float sphereRadius ,
		float3 rayStart ,
		float3 rayEnd ,
		out RaycastHit raycastHit
	)
	{
		var ray = new RaycastInput{
			Start	= rayStart ,
			End		= rayEnd ,
			Filter	= CollisionFilter.Default
		};
		var geometry  = new SphereGeometry{
			Center	= spherePos ,
			Radius	= sphereRadius
		};
		var sphere = new SphereCollider();
		sphere.Initialize( geometry , CollisionFilter.Default , Material.Default );
		return sphere.CastRay( ray , out raycastHit );
	}

}

public static class ExtensionMethods_TraceInternals
{

	public static void Cast (
		this Trace.ITrace<Trace.SphereDat,Trace.RayDat> trace ,
		out bool didHit ,
		out RaycastHit raycastHit
	)
	{
		var sphere = trace.a;
		var ray = trace.b;
		didHit = Trace.SphereRay(
			spherePos:		sphere.position ,
			sphereRadius:	sphere.radius ,
			rayStart:		ray.origin ,
			rayEnd:			ray.origin + ray.dir * 10000f ,
			out raycastHit
		);
	}
	public static bool Cast (
		this Trace.ITrace<Trace.SphereDat,Trace.RayDat> trace ,
		out RaycastHit raycastHit
	)
	{
		var sphere = trace.a;
		var ray = trace.b;
		return Trace.SphereRay(
			spherePos:		sphere.position ,
			sphereRadius:	sphere.radius ,
			rayStart:		ray.origin ,
			rayEnd:			ray.origin + ray.dir * 10000f ,
			out raycastHit
		);
	}
	public static bool Cast ( this Trace.ITrace<Trace.SphereDat,Trace.RayDat> trace )
	{
		var sphere = trace.a;
		var ray = trace.b;
		return Trace.SphereRay(
			spherePos:		sphere.position ,
			sphereRadius:	sphere.radius ,
			rayStart:		ray.origin ,
			rayEnd:			ray.origin + ray.dir * 1e3f ,
			out var raycastHit
		);
	}

	public static bool Overlap ( this Trace.ITrace<Trace.SphereDat,Trace.SphereDat> trace )
	{
		var a = trace.a;
		var b = trace.b;
		return math.lengthsq( a.position - b.position ) < math.pow( a.radius+b.radius , 2f );
	}

	public static Trace.TraceDat<A,Trace.RayDat> Ray <A> ( this A a , float3 origin , float3 dir )
		where A : Trace.ITraceObject
		=> new Trace.TraceDat<A,Trace.RayDat>( a:a , b:new Trace.RayDat( origin:origin , dir:dir ) );
	public static Trace.TraceDat<A,Trace.RayDat> Ray <A> ( this A a , (float x,float y, float z) origin , (float x,float y, float z) dir )
		where A : Trace.ITraceObject
		=> new Trace.TraceDat<A,Trace.RayDat>( a:a , b:new Trace.RayDat( origin:new float3{ x=origin.x , y=origin.y , z=origin.z } , dir:new float3{ x=dir.x , y=dir.y , z=dir.z } ) );
	public static Trace.TraceDat<A,Trace.RayDat> Ray <A> ( this A a , UnityEngine.Ray ray )
		where A : Trace.ITraceObject
		=> new Trace.TraceDat<A,Trace.RayDat>( a:a , b:new Trace.RayDat( origin:ray.origin , dir:ray.direction ) );

	public static Trace.TraceDat<A,Trace.SphereDat> Sphere <A> ( this A a , float3 pos , float r )
		where A : Trace.ITraceObject
		=> new Trace.TraceDat<A,Trace.SphereDat>( a:a , b:new Trace.SphereDat( pos:pos , r:r ) );
	public static Trace.TraceDat<A,Trace.SphereDat> Sphere <A> ( this A a , (float x,float y, float z) pos , float r )
		where A : Trace.ITraceObject
		=> new Trace.TraceDat<A,Trace.SphereDat>( a:a , b:new Trace.SphereDat( pos:new float3{ x=pos.x , y=pos.y , z=pos.z } , r:r ) );

}
