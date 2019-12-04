using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SteeringAgent : MonoBehaviour
{
	public GameObject _seekTarget;
	public virtual GameObject seekTarget
	{
		get
		{
			return _seekTarget;
		}
		set
		{
			_seekTarget = value;
		}
	}

	protected List<GameObject> _obstacles;
	protected HashSet<GameObject> _superAvoid = new HashSet<GameObject>();
	public virtual List<GameObject> obstacles
	{
		get
		{
			return _obstacles;
		}
	}

	public virtual List<GameObject> fleeObstacles
	{
		get
		{
			List<GameObject> all = new List<GameObject>();
			all.AddRange(AgentManager.Instance.shoppers);
			all.AddRange(AdvertiserManager.Instance.advertisers);
			return all;
		}
	}
	List<GameObject> chairs
	{
		get
		{
			return WorldManager.Instance.chairs;
		}
	}
	List<GameObject> shops
	{
		get
		{
			return WorldManager.Instance.shops;
		}
	}

	public virtual float maxVelocity
	{
		get { return 2f; }
	}
	public virtual float maxSeekForce
	{
		get { return 5.5f; }
	}
	public virtual float maxAvoidForce
	{
		get { return 10.5f; }
	}

	public Vector2 velocity
	{
		get
		{
			return rb.velocity;
		}
	}

	public virtual float desiredSeperation
	{
		get { return 1f; }
	}

	public virtual float maxFleeForce
	{
		get { return 30.5f; }
	}

	Vector2 steering;

	Rigidbody2D rb;

	public bool waiting;

	// Testing
	bool test = false;
	public GameObject testTarget;
	public Material threatMaterial;
	public Material nonThreatMaterial;


	// Start is called before the first frame update
	protected void Start()
	{
		foreach(GameObject p in WorldManager.Instance.planters)
		{
			_superAvoid.Add(p);
		}
		if (test)
		{
			_seekTarget = testTarget;
		}
		_obstacles = WorldManager.Instance.GetAllObstacles();
		//velocity = new Vector2(0, 0);
		rb = gameObject.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	public virtual void Update()
	{
		Move();
	}

	void Move()
	{
		if (!waiting)
		{
			steering = GetSeekForce() + GetTotalAvoidForce() + GetTotalFleeForce();
			rb.velocity += steering;
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	// Stand still for seconds until resuming movement
	protected IEnumerator PauseMovement(float seconds)
	{
		waiting = true;
		yield return new WaitForSeconds(seconds);
		waiting = false;
	}

	// Return the seek force towards the current target
	Vector2 GetSeekForce()
	{
		if (!seekTarget) return Vector2.zero;
		Vector2 desiredVelocity = seekTarget.transform.position - transform.position;
		desiredVelocity = desiredVelocity.normalized * maxVelocity;
		steering = desiredVelocity - velocity;
		return Vector2.ClampMagnitude(steering, maxSeekForce);
	}

	// Return the force to push away from the given obstacle
	Vector2 GetAvoidForce(GameObject obstacle)
	{
		const float THRESHOLD = 1f;
		if (!obstacle)
		{
			return Vector2.zero;
		}
		float distance = gameObject.GetComponentInChildren<Collider2D>().Distance(obstacle.gameObject.GetComponentInChildren<Collider2D>()).distance;

		Vector2 adj = obstacle.transform.position - transform.position; // Adjustment to be made
		float diff = THRESHOLD - distance;

		if(distance > THRESHOLD) // Ignore objects further than the threshold
		{
			if(test) obstacle.GetComponentInChildren<Renderer>().material = nonThreatMaterial;
			return Vector2.zero;
		}

		if (test)
		{
			obstacle.GetComponentInChildren<Renderer>().material = threatMaterial;
		}
		adj.Normalize();
		adj = adj * diff; // Scale adjustment based on distance from obstacle

		return -adj; // Return negative to steer away
	}

	Vector2 GetTotalAvoidForce()
	{
		if (!seekTarget) return Vector2.zero;

		Vector2 avoidance = Vector2.zero;
		foreach(GameObject obstacle in obstacles)
		{
			if(obstacle == seekTarget && !_superAvoid.Contains(seekTarget))
			{
				continue;
			}
			if (!_superAvoid.Contains(obstacle))
			{
				avoidance += GetAvoidForce(obstacle);
			}
			else
			{
				avoidance += 2 * GetAvoidForce(obstacle);
			}
		}

		return Vector2.ClampMagnitude(avoidance, maxAvoidForce);
	}

	protected Vector2 GetFleeForce(GameObject obstacle, bool superAvoid=false)
	{
		if (!obstacle)
		{
			return Vector2.zero;
		}
		float distance = Vector2.Distance(obstacle.transform.position, transform.position);
		float fleeDistance = superAvoid ? desiredSeperation * 3 : desiredSeperation;
		if (distance > 0 && distance < fleeDistance)
		{
			Vector2 diff = obstacle.transform.position - transform.position;
			return -diff.normalized;
		}
		else
		{
			return Vector2.zero;
		}
	}

	protected virtual Vector2 GetTotalFleeForce()
	{
		Vector2 flee = Vector2.zero;
		foreach(GameObject obstacle in fleeObstacles)
		{
			if(obstacle != gameObject && obstacle != seekTarget)
			{
				flee += _superAvoid.Contains(obstacle) ? GetFleeForce(obstacle, true) : GetFleeForce(obstacle, false);
			}
		}

		return Vector2.ClampMagnitude(flee, maxFleeForce);
	}


}
