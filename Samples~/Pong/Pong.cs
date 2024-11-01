using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Levers.Samples
{
    public class Pong : EditorWindow
    {
        [MenuItem("Window/Levers/Samples/Games/Pong")]
        public static void ShowWindow()
        {
            _ = GetWindow<Pong>("Pong");
        }

        private double _lastTime;

        private AIAgent MakeDefaultAIAgent1()
        {
            return new AIAgent(this, () => _player1.y, -1);
        }

        private Image _centerLine;

        private void OnEnable()
        {
            _lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += UpdateAnimation;
            _player1 = new PlayerState
            {
                y = 0.5f,
                x = _padding + (_paddleThickness / 2f),
                agent = MakeDefaultAIAgent1()
            };
            _player2 = new PlayerState
            {
                y = 0.5f,
                x = 1f - _padding - (_paddleThickness / 2f),
                agent = new AIAgent(this, () => _player2.y, 1)
            };
            var centerLineTexture = new Texture2D(1, 2);
            centerLineTexture.SetPixel(0, 0, Color.white);
            centerLineTexture.SetPixel(0, 1, Color.black);
            centerLineTexture.Apply();
            centerLineTexture.filterMode = FilterMode.Point;
            _centerLine = new Image(centerLineTexture);
        }

        private void UpdateAnimation()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - _lastTime);
            _lastTime = currentTime;
            UpdateState(deltaTime);
            Repaint();
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateAnimation;
        }

        private struct PlayerState
        {
            public float y;
            public float score;
            public float x;
            public PlayerAgent agent;
        }

        private abstract class PlayerAgent
        {
            public abstract bool UpPressed { get; }
            public abstract bool DownPressed { get; }
        }

        private class AIAgent : PlayerAgent
        {
            private readonly Pong _game;
            private readonly Func<float> _playerY;
            private readonly int _direction;

            public override bool UpPressed => Mathf.Sign(_game._ballDirection.x) == _direction && _game._ballPosition.y < _playerY();
            public override bool DownPressed => Mathf.Sign(_game._ballDirection.x) == _direction && _game._ballPosition.y > _playerY();
            public AIAgent(Pong game, Func<float> playerY, int direction)
            {
                _game = game;
                _playerY = playerY;
                _direction = direction;
            }
        }

        private class HumanAgent : PlayerAgent
        {
            public override bool UpPressed => _upPressed;
            public override bool DownPressed => _downPressed;
            private readonly HashSet<KeyCode> _upKeys;
            private readonly HashSet<KeyCode> _downKeys;
            private bool _upPressed;
            private bool _downPressed;

            public HumanAgent(IEnumerable<KeyCode> upKeys = null, IEnumerable<KeyCode> downKeys = null)
            {
                _upKeys = upKeys != null ? new HashSet<KeyCode>(upKeys) : new HashSet<KeyCode> { KeyCode.W, KeyCode.UpArrow };
                _downKeys = downKeys != null ? new HashSet<KeyCode>(downKeys) : new HashSet<KeyCode> { KeyCode.S, KeyCode.DownArrow };
            }

            public void HandleKeyDown()
            {
                if (_upKeys.Contains(Event.current.keyCode))
                {
                    _upPressed = true;
                    Event.current.Use();
                }
                else if (_downKeys.Contains(Event.current.keyCode))
                {
                    _downPressed = true;
                    Event.current.Use();
                }
            }

            public void HandleKeyUp()
            {
                if (_upKeys.Contains(Event.current.keyCode))
                {
                    _upPressed = false;
                    Event.current.Use();
                }
                else if (_downKeys.Contains(Event.current.keyCode))
                {
                    _downPressed = false;
                    Event.current.Use();
                }
            }
        }

        private const float _paddleWidth = 0.2f;
        private const float _ballSpeed = 0.55f;
        private const float _paddleSpeed = 0.3f;
        private const float _paddleThickness = 0.025f;
        private const float _padding = 0.05f;
        private const float _ballRadius = 0.02f;
        private PlayerState _player1;
        private PlayerState _player2;
        private Vector2 _ballPosition = new Vector2(0.5f, 0.5f);
        private Vector2 _ballDirection = new Vector2(1f, 1f).normalized;
        private bool _afterFirstHit;

        private void ResetBall()
        {
            _ballPosition = new Vector2(0.5f, 0.5f);
            var dir = UnityEngine.Random.insideUnitCircle;
            dir.x = Mathf.Abs(dir.x) < 0.2f ? 0.2f * Mathf.Sign(dir.x) : dir.x;
            _ballDirection = dir.normalized;
            _afterFirstHit = false;
        }

        private float? PaddleIntersection(float paddleX, float paddleY, Vector2 ballStart, Vector2 ballEnd)
        {
            return ComputeXIntersection(ballStart, ballEnd, paddleX) is float y && y > paddleY - (_paddleWidth / 2f) && y < paddleY + (_paddleWidth / 2f)
                ? y
                : (float?)null;
        }

        private static float? ComputeXIntersection(Vector2 pointA, Vector2 pointB, float x)
        {
            var crossesX = (pointA.x > x) != (pointB.x > x);
            if (!crossesX)
            {
                return null;
            }
            var m = (pointB.y - pointA.y) / (pointB.x - pointA.x);
            var b = pointA.y - (m * pointA.x);
            var y = (m * x) + b;
            return ((pointA.y > y) != (pointB.y > y)) ? y : (float?)null;
        }

        private static float? ComputeYIntersection(Vector2 pointA, Vector2 pointB, float y)
        {
            var crossesY = (pointA.y > y) != (pointB.y > y);
            if (!crossesY)
            {
                return null;
            }
            var m = (pointB.y - pointA.y) / (pointB.x - pointA.x);
            var b = pointA.y - (m * pointA.x);
            var x = (y - b) / m;
            return ((pointA.x > x) != (pointB.x > x)) ? x : (float?)null;
        }

        private void UpdateState(float deltaTime)
        {
            if ((_player1.agent is HumanAgent) != _enablePlayerControl)
            {
                _player1.agent = _enablePlayerControl ? (PlayerAgent)new HumanAgent() : MakeDefaultAIAgent1();
            }

            void UpdatePaddlePosition(ref PlayerState player)
            {
                if (player.agent.UpPressed)
                {
                    player.y -= deltaTime * _paddleSpeed;
                }
                else if (player.agent.DownPressed)
                {
                    player.y += deltaTime * _paddleSpeed;
                }

                if (player.y + (_paddleWidth / 2f) > 1f)
                {
                    player.y = 1f - (_paddleWidth / 2f);
                }
                else if (player.y - (_paddleWidth / 2f) < 0f)
                {
                    player.y = 0f + (_paddleWidth / 2f);
                }
            }
            UpdatePaddlePosition(ref _player1);
            UpdatePaddlePosition(ref _player2);

            var travelDistance = deltaTime * _ballSpeed * (_afterFirstHit ? 1.1f : 0.8f);
            var newPosition = _ballPosition + (_ballDirection * travelDistance);

            void HandleBounce(Vector2 bouncePosition, bool x, Vector2 offset, float english = 0f)
            {
                var rayOrigin = bouncePosition;
                var distanceToBouncePosition = (bouncePosition - rayOrigin).magnitude;
                var newDirection = ((x ? new Vector2(1, -1) : new Vector2(-1, 1)) * _ballDirection);
                newDirection = Vector2.Lerp(newDirection, new Vector2(Mathf.Sign(newDirection.x) * 0.15f, Mathf.Sign(english)), Mathf.Abs(english));
                newDirection = newDirection.normalized;
                var newPositionAfterBounce = bouncePosition + (newDirection * (travelDistance - distanceToBouncePosition));
                _ballPosition = newPositionAfterBounce - offset;
                _ballDirection = newDirection;
            }

            void HandleBounceY(Vector2 bouncePosition, Vector2 offset, float english)
            {
                HandleBounce(bouncePosition, false, offset, english);
            }

            void HandleBounceX(Vector2 bouncePosition, Vector2 offset)
            {
                HandleBounce(bouncePosition, true, offset);
            }

            var topLeftOffset = new Vector2(-_ballRadius, -_ballRadius);
            var topRightOffset = new Vector2(_ballRadius, -_ballRadius);
            var bottomLeftOffset = new Vector2(-_ballRadius, _ballRadius);
            var bottomRightOffset = new Vector2(_ballRadius, _ballRadius);
            var topOffset = new Vector2(0f, -_ballRadius);
            var bottomOffset = new Vector2(0f, _ballRadius);

            if (PaddleIntersection(_player1.x, _player1.y, _ballPosition + topLeftOffset, newPosition + topLeftOffset) is float player1IntersectionT)
            {
                _afterFirstHit = true;
                var english = (_ballPosition.y - _player1.y) / (_paddleWidth / 2f);
                HandleBounceY(new Vector2(_player1.x, player1IntersectionT), topLeftOffset, english);
            }
            else if (PaddleIntersection(_player1.x, _player1.y, _ballPosition + bottomLeftOffset, newPosition + bottomLeftOffset) is float player1IntersectionB)
            {
                _afterFirstHit = true;
                var english = (_ballPosition.y - _player1.y) / (_paddleWidth / 2f);
                HandleBounceY(new Vector2(_player1.x, player1IntersectionB), bottomLeftOffset, english);
            }
            else if (PaddleIntersection(_player2.x, _player2.y, _ballPosition + topRightOffset, newPosition + topRightOffset) is float player2IntersectionT)
            {
                _afterFirstHit = true;
                var english = (_ballPosition.y - _player2.y) / (_paddleWidth / 2f);
                HandleBounceY(new Vector2(_player2.x, player2IntersectionT), topRightOffset, english);
            }
            else if (PaddleIntersection(_player2.x, _player2.y, _ballPosition + bottomRightOffset, newPosition + bottomRightOffset) is float player2IntersectionB)
            {
                _afterFirstHit = true;
                var english = (_ballPosition.y - _player2.y) / (_paddleWidth / 2f);
                HandleBounceY(new Vector2(_player2.x, player2IntersectionB), bottomRightOffset, english);
            }
            else if (ComputeYIntersection(_ballPosition + bottomOffset, newPosition + bottomOffset, 1f) is float bottomWallIntersectionXL)
            {
                HandleBounceX(new Vector2(bottomWallIntersectionXL, 1f), bottomOffset);
            }
            else if (ComputeYIntersection(_ballPosition + topOffset, newPosition + topOffset, 0f) is float topWallIntersectionXL)
            {
                HandleBounceX(new Vector2(topWallIntersectionXL, 0f), topOffset);
            }
            else if (newPosition.x < 0)
            {
                _player2.score++;
                ResetBall();
            }
            else if (newPosition.x > 1)
            {
                _player1.score++;
                ResetBall();
            }
            else
            {
                _ballPosition = newPosition;
            }

        }

        private void Render(Rect position)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.Repaint:
                    {
                        Draw.TextureFill = null;
                        Draw.Stroke = Color.clear;
                        Draw.Fill = Color.black;
                        Draw.Rect(position);
                        /* Center line */
                        {
                            Draw.Stroke = Color.clear;
                            Draw.Fill = Color.white;
                            Draw.TextureFill = _centerLine;
                            _centerLine.Scale = new Vector2(8, 8);
                            const float thickness = 0.02f;
                            Draw.Rect(new Rect(
                                x: position.x + (position.width / 2f) - (thickness / 2f),
                                y: position.y,
                                width: thickness * position.width,
                                height: position.height
                                ));
                            Draw.TextureFill = null;
                        }
                        void DrawPaddle(PlayerState player)
                        {
                            Draw.Fill = Color.white;
                            var paddleRect = new Rect(
                                x: position.x + ((player.x - (_paddleThickness / 2f)) * position.width),
                                y: position.y + ((player.y - (_paddleWidth / 2f)) * position.height),
                                width: _paddleThickness * position.width,
                                height: _paddleWidth * position.height
                                );
                            Draw.Rect(paddleRect);
                        }
                        void DrawScore(PlayerState player, bool left)
                        {
                            var scoreText = player.score.ToString();
                            var scoreRect = new Rect(
                                x: position.x + (left ? 0 : position.width / 2f),
                                y: position.y,
                                width: position.width / 2f,
                                height: position.height * 0.1f
                                );
                            GUI.Label(scoreRect, scoreText, new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                            {
                                fontSize = (int)(scoreRect.height * 0.8f),
                            });
                        }
                        DrawPaddle(_player1);
                        DrawPaddle(_player2);
                        Draw.Fill = Color.white;
                        var ballRect = new Rect(
                            x: position.x + ((_ballPosition.x - _ballRadius) * position.width),
                            y: position.y + ((_ballPosition.y - _ballRadius) * position.height),
                            width: _ballRadius * 2 * position.width,
                            height: _ballRadius * 2 * position.height
                            );
                        Draw.Rect(ballRect);
                        DrawScore(_player1, true);
                        DrawScore(_player2, false);
                        break;
                    }
                case EventType.MouseDown:
                    {
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                        break;
                    }
                case EventType.KeyDown:
                    {
                        (_player1.agent as HumanAgent)?.HandleKeyDown();
                        (_player2.agent as HumanAgent)?.HandleKeyDown();
                        break;
                    }
                case EventType.KeyUp:
                    {
                        (_player1.agent as HumanAgent)?.HandleKeyUp();
                        (_player2.agent as HumanAgent)?.HandleKeyUp();
                        break;
                    }
                default:
                    break;
            }
        }

        private bool _enablePlayerControl = false;

        private void OnGUI()
        {
            var rect = new Rect(Vector2.zero, position.size);
            _enablePlayerControl = GUI.Toggle(new Rect(rect.position, new Vector2(rect.width, EditorGUIUtility.singleLineHeight)), _enablePlayerControl, "Enable Player Control");
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height -= EditorGUIUtility.singleLineHeight;
            Render(rect);
        }
    }
}
