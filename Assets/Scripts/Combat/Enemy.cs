using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Enemy : GenericEnemy
    {
        [Header("Movement")]
        [SerializeField]
        private float speed = 3.5f;
        [SerializeField]
        private float personalSpace = 1f;
        [SerializeField]
        private LayerMask wallLayer;
        [SerializeField]
        private string searchMode = "";

        [Header("Combat")]
        [SerializeField]
        private GameObject projectilePrefab;
        [SerializeField]
        private float attackRange = 1f;
        [SerializeField]
        private float seeRange = 15f;
        [SerializeField]
        private float fireCooldown = .5f;
        [SerializeField]
        private float nearAttackDmg = 1f;
        [SerializeField]
        private float rangedAttackDmg = 1f;
        [SerializeField]
        private float prjSpeed = 1f;

        [Header("Audio Settings")]
        [SerializeField]
        private float minDist = 1;
        [SerializeField]
        private float maxDist = 20;
        [SerializeField]
        private float maxVolume = 0.8f;
        [SerializeField]
        private AudioSource footsteps;

        private bool alive = true;

        private float nextFire = .0f;
        private bool isMoving = false;

        private Animator characterAnimator;
        private Rigidbody2D rb2d;
        private BoxCollider2D bc2d;

        private void Awake()
        {
            characterAnimator = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            bc2d = GetComponent<BoxCollider2D>();
            if (target == null)
            {
                target = GameObject.FindWithTag(Keys.TAG_PLAYER);
            }
        }

        void Update()
        {
            var movement = HandleMovement();
            HandleAnimation(movement);
            handleSound();
            handleVolume();

            if (!alive && !(characterAnimator.GetCurrentAnimatorStateInfo(0).length > characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime))
            {
                Destroy(this.gameObject);
            }
        }

        void FixedUpdate()
        {
            HandleCombat();
        }

        Vector3 HandleMovement()
        {
            if (!alive)
            {
                isMoving = false;
                return transform.position;
            }

            if (target == null)
            {
                isMoving = false;
                searchMode = "no enemy";
                return transform.position;
            }
            var moveTarget = transform.position;

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > seeRange)
            {
                searchMode = "no enemy in sight";
                isMoving = false;
            }
            else if (distance < attackRange && distance > personalSpace)
            {
                searchMode = "attacking";
                isMoving = false;
            }
            else
            {
                // Determine where to move to
                if (distance < personalSpace)
                {
                    // Flee in opposite direction
                    moveTarget = transform.position - (target.transform.position - transform.position).normalized;
                    searchMode = "fleeing";
                }
                else
                {
                    moveTarget = target.transform.position;
                    searchMode = "approaching";
                }

                moveTarget = MoveToTarget(moveTarget, distance);
                isMoving = moveTarget != transform.position;
            }

            var movement = (moveTarget - transform.position).normalized * Time.deltaTime * speed;
            rb2d.MovePosition(transform.position + movement);

            return movement;
        }

        /// <summary>
        /// Find a sensible way to move towards a given target, respecting walls, edges and distance
        /// </summary>
        /// <param name="moveTarget"></param>
        /// <param name="distance"></param>
        /// <returns>The position the character is actually supposed to move to now.</returns>
        private Vector3 MoveToTarget(Vector3 moveTarget, float distance)
        {
            // Find a way
            // Only the walls of the tile map / blocking layer are interesting
            RaycastHit2D hitInfo = Physics2D.BoxCast(transform.position, bc2d.size, 0, moveTarget - transform.position, distance, wallLayer);
            Debug.DrawRay(transform.position, moveTarget - transform.position, Color.red);
            if (hitInfo.collider != null && hitInfo.distance < distance)
            {
                var fCord = new Coordinate(transform.position);
                var tCord = new Coordinate(moveTarget);

                // Abort for glitchy cases for speedup
                if (map[tCord.x, tCord.y] != AlgorithmUtils.floorTile || map[fCord.x, fCord.y] != AlgorithmUtils.floorTile)
                {
                    Debug.LogWarning("Glitchy path found");
                    searchMode += "/glitchy";
                    return transform.position;
                }

                // Limit here the max cost to not hog cpu for far enemies
                List<Coordinate> path = AStarAlgorithm.AStar(fCord, tCord, map, mapWidth, mapHeight, true, 2000);

                // A way has been found
                if (path.Count > 1)
                {
                    // We draw the way to analyse the movement path chosen
                    var lastCord = path[0];
                    foreach (var cord in path)
                    {
                        Debug.DrawLine(lastCord.ToCentralPosition(), cord.ToCentralPosition(), Color.cyan);
                        lastCord = cord;
                    }

                    // Move towards the next step of the path
                    // The first tile is the starting tile, so use second
                    Coordinate targetTile = path[1];

                    // Manage diagonal moves, as enemies will stop at walls otherwise
                    if (targetTile.x != fCord.x && targetTile.y != fCord.y)
                    {
                        if (map[targetTile.x, fCord.y] == AlgorithmUtils.wallTile)
                        {
                            targetTile = new Coordinate(fCord.x, targetTile.y);
                        }
                        else if (map[fCord.x, targetTile.y] == AlgorithmUtils.wallTile)
                        {
                            targetTile = new Coordinate(targetTile.x, fCord.y);
                        }
                    }

                    // In case we cannot reach this tile we probably have to move more to the center of the "current" tile
                    hitInfo = Physics2D.BoxCast(transform.position, bc2d.size, 0, targetTile.ToCentralPosition() - transform.position, 0.3f, wallLayer);
                    if (hitInfo.collider != null)
                    {
                        // We would want to go here, as soon as we can reach it
                        Debug.DrawLine(transform.position, targetTile.ToCentralPosition(), Color.yellow);

                        targetTile = path[0];
                    }

                    moveTarget = targetTile.ToCentralPosition();

                    Debug.DrawLine(transform.position, moveTarget, Color.blue);

                    searchMode += "/" + path.Count + " tiles";
                }
                else
                {
                    moveTarget = transform.position;
                    searchMode += "/no way";
                }
                searchMode += "/searching";
            }
            else
            {
                moveTarget = target.transform.position;

                // We draw the way to analyse the movement path chosen
                Debug.DrawLine(transform.position, moveTarget, Color.blue);

                searchMode += "/direct";
            }
            return moveTarget;
        }

        private void handleSound()
        {
            if (isMoving && !footsteps.isPlaying)
            {
                // float volume =  Mathf.Clamp(1.0f-(distance/10f),0f,1.0f);
                footsteps.Play();
            }
            else
            {
                footsteps.Pause();
            }
        }

        /// <summary>
        /// @author: Neele Kemper
        /// Fade 2D audio by distance
        /// </summary>
        /// <returns></returns>
        private void handleVolume()
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < minDist)
            {
                footsteps.volume = maxVolume;
            }
            else if (dist > maxDist)
            {
                footsteps.volume = 0;
            }
            else
            {
                footsteps.volume = maxVolume - ((dist - minDist) / (maxDist - minDist));
            }
        }

        void HandleAnimation(Vector3 movement)
        {
            characterAnimator.SetFloat(Keys.ANIMATION_SPEED_KEY, movement.magnitude / Time.deltaTime);
            if (movement.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else if (movement.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private void HandleCombat()
        {
            if (target == null)
                return;

            float distance = Vector3.Distance(transform.position, target.transform.position);

            // Nothing to do if on cooldown or out of distance or not ranged enemy
            if (distance > attackRange || Time.time < nextFire)
                return;

            // Refresh cd
            nextFire = Time.time + fireCooldown;

            // Shoot projectile
            var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            var projectileController = projectile.GetComponent<Projectile>();
            projectileController.damage = rangedAttackDmg;
            projectileController.speed = prjSpeed;

            // Aim projectile
            var relative = projectile.transform.InverseTransformPoint(target.transform.position);
            float angle = 90 - Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            projectile.transform.Rotate(0, 0, angle);
        }

        public override void OnHitPlayer(Player player)
        {
            if (!alive)
            {
                return;
            }

            // Close combat
            player.damage(Mathf.CeilToInt(nearAttackDmg));
        }

        public override void OnBeingHit()
        {
            if (!alive)
            {
                return;
            }

            alive = false;
            characterAnimator.SetTrigger(Keys.ANIMATION_DEAD_KEY);
        }
    }
}
