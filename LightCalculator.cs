using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using NCalc;

namespace LightCalculator
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class LightCalculator : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont _policeEcriture;
        public List<Tuple<string, Rectangle>> ListeRectangleBouton;
        public List<Tuple<string, string>> ListeHistoriqueCalcul;
        public Rectangle RectangleCalcul;
        public Rectangle RectangleResultat;
        public Rectangle RectangleBoutonViderHistorique;
        public Rectangle RectangleBoutonRetourHistorique;
        public Rectangle RectangleBoutonInterrogation;
        private DisplayOrientation _orientationEcran;
        private string _calcul;
        private string _calculGraphique;
        private float _resultat;
        private int _ecartResultatCalcul;
        private bool _generationAffichage;
        private bool _affichageHistorique;
        private bool _detectionScroll;
        private float _defilementCalcul;
        private int _directionDefilement;

        #region Variable des textures.

        private Texture2D _boutonNormalTexture;
        private Texture2D _boutonEgaleTexture;
        private Texture2D _boutonAideTexture;

        #endregion

        /// <summary>
        /// Constructeur de la class.
        /// </summary>
        public LightCalculator()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                SupportedOrientations = DisplayOrientation.Portrait,
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferMultiSampling = false,
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
            };
            graphics.ToggleFullScreen();
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            ListeRectangleBouton = new List<Tuple<string, Rectangle>>();
            ListeHistoriqueCalcul = new List<Tuple<string, string>>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            using (var stream = TitleContainer.OpenStream("Content/Bouton/bouton.png"))
            {
                _boutonNormalTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }

            using (var stream = TitleContainer.OpenStream("Content/Bouton/bouton-egale.png"))
            {
                _boutonEgaleTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }

            using (var stream = TitleContainer.OpenStream("Content/Bouton/bouton-aide.png"))
            {
                _boutonAideTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }

            _policeEcriture = Content.Load<SpriteFont>("Ecriture");

        }


        /// <summary>
        /// Calcul la position des boutons en fonction de l'interface graphique.
        /// </summary>
        private void CalculPositionGraphique()
        {
            _orientationEcran = Window.CurrentOrientation;

            int basePositionBoutonY = (GraphicsDevice.Viewport.Height * 30) / 100;
            int basePositionCalcul = (GraphicsDevice.Viewport.Height * 5) / 100;


            int basePositionResultat = (GraphicsDevice.Viewport.Height * 20) / 100;

            int largeurBoutonAide = 0;

            if (_orientationEcran == DisplayOrientation.Portrait ||
                _orientationEcran == DisplayOrientation.PortraitDown)
            {
                largeurBoutonAide = (GraphicsDevice.Viewport.Width * 7) / 100;
            }
            else
            {
                largeurBoutonAide = ((GraphicsDevice.Viewport.Width * 7) / 100) / 2;
            }

            int hauteurBoutonAide = largeurBoutonAide;

            _ecartResultatCalcul = basePositionResultat - basePositionCalcul;


            RectangleBoutonViderHistorique = new Rectangle(0, ((GraphicsDevice.Viewport.Height * 90) / 100),
                GraphicsDevice.Viewport.Width,
                ((GraphicsDevice.Viewport.Height) * 10 / 100));

            RectangleBoutonRetourHistorique = new Rectangle(0,
                RectangleBoutonViderHistorique.Y - RectangleBoutonViderHistorique.Height,
                RectangleBoutonViderHistorique.Width, RectangleBoutonViderHistorique.Height);

            RectangleCalcul = new Rectangle(0, basePositionCalcul, GraphicsDevice.Viewport.Width,
                ((basePositionResultat * 90 / 100)));
            RectangleResultat = new Rectangle(0, basePositionResultat, GraphicsDevice.Viewport.Width, 25);

            RectangleBoutonInterrogation =
                new Rectangle((int) (GraphicsDevice.Viewport.Width - (largeurBoutonAide * 1.5f)), hauteurBoutonAide/2, largeurBoutonAide,
                    hauteurBoutonAide);

            int largeurBouton = 0;
            int hauteurBouton = 0;
            if (_orientationEcran == DisplayOrientation.Portrait ||
                               _orientationEcran == DisplayOrientation.PortraitDown)
            {
                largeurBouton = (GraphicsDevice.Viewport.Width * 25 / 100);

                
                hauteurBouton = ((GraphicsDevice.Viewport.Height * 70 / 100) / 5);
            }
            else
            {
                largeurBouton = (GraphicsDevice.Viewport.Width * 25 / 100);
                hauteurBouton = (int)(GraphicsDevice.Viewport.Height * 12.5f / 100); ;
            }


            List<string> listeBouton = new List<string>
            {
                "C",
                "(",
                ")",
                "<-",
                "/",
                "1",
                "2",
                "3",
                "x",
                "4",
                "5",
                "6",
                "-",
                "7",
                "8",
                "9",
                "+",
                "0",
                ".",
                "Hist"
            };

            int hauteurJump = basePositionBoutonY;
            ListeRectangleBouton.Clear();
            foreach (var t in listeBouton)
            {
                if (ListeRectangleBouton.Count > 0)
                {
                    if (ListeRectangleBouton[ListeRectangleBouton.Count - 1].Item2.X + largeurBouton >=
                        GraphicsDevice.Viewport.Width)
                    {
                        hauteurJump += hauteurBouton;
                        ListeRectangleBouton.Add(new Tuple<string, Rectangle>(t, new Rectangle(0, hauteurJump,
                            largeurBouton, hauteurBouton)));
                    }
                    else
                    {
                        ListeRectangleBouton.Add(new Tuple<string, Rectangle>(t, new Rectangle(largeurBouton + ListeRectangleBouton[ListeRectangleBouton.Count - 1].Item2.X, hauteurJump,
                            largeurBouton, hauteurBouton)));
                    }
                }
                else
                {
                    Rectangle rectangleBouton = new Rectangle(0, hauteurJump, largeurBouton, hauteurBouton);
                    ListeRectangleBouton.Add(new Tuple<string, Rectangle>(t, rectangleBouton));
                }
            }

        }




        private int _compteurClique;

        /// <summary>
        /// Execute les fonctions de calcul des touches tactile.
        /// </summary>
        private void TouchFonction()
        {
            var etatSouris = TouchPanel.GetState();

            foreach (var t in etatSouris)
            {
                if (t.State == TouchLocationState.Pressed)
                {
                    _compteurClique++;



                    if (_compteurClique >= 1)
                    {


                        if (_affichageHistorique == false)
                        {
                            if (RectangleCalcul.Contains(t.Position))
                            {
                                Task.Factory.StartNew(delegate()
                                    {
                                        try
                                        {
                                            string tmpCalcul = _calcul.Replace("*", "x");
                                            float testCalcul = 0;
                                            tmpCalcul =
                                                KeyboardInput.Show("Editer votre calcul ",
                                                    "Editer votre calcul plus facilement",
                                                    tmpCalcul, false).Result;
                                            tmpCalcul = tmpCalcul.Replace("x", "*");
                                            tmpCalcul = tmpCalcul.Replace("X", "*");
                                            tmpCalcul = tmpCalcul.Replace("÷", "/");

                                            bool bonCalcul;
                                            try
                                            {

                                                Expression computeCalculation = new Expression(tmpCalcul);

                                               
                                                    testCalcul =
                                                        float.Parse(computeCalculation.Evaluate().ToString());
                                                    bonCalcul = true;
                                                
                                            }
                                            catch
                                            {
                                                MessageBox.Show("Erreur de calcul",
                                                    "Votre calcul comporte des erreurs.",
                                                    new[] {"OK"});
                                                bonCalcul = false;
                                            }

                                            if (bonCalcul)
                                            {
                                                _resultat = testCalcul;
                                                _calculGraphique = "";
                                                string tmpCalculGraphique = "";
                                                for (int i = 0; i < tmpCalcul.Length; i++)
                                                {
                                                    if (i < tmpCalcul.Length)
                                                    {
                                                        tmpCalculGraphique += tmpCalcul[i];
                                                        string tmpCalcu = tmpCalculGraphique.Substring(i);
                                                        var fontSize = _policeEcriture.MeasureString(tmpCalcu);
                                                        if (fontSize.X / 1.35f > GraphicsDevice.Viewport.Width)
                                                        {
                                                            tmpCalculGraphique = tmpCalculGraphique.Insert(i, "\n");
                                                        }

                                                    }
                                                }

                                                _calculGraphique = tmpCalculGraphique.Replace("*", "x");
                                                _calculGraphique = _calculGraphique.Replace("÷", "/");
                                                _calcul = tmpCalcul;
                                            }

                                        }
                                        catch (Exception erreur)
                                        {
#if DEBUG
                                        Console.WriteLine("Erreur: "+erreur.Message);
#endif
                                        }
                                    }, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously,
                                    TaskScheduler.Current);
                            }

                            if (RectangleBoutonInterrogation.Contains(t.Position))
                            {
                                Task.Factory.StartNew(delegate()
                                    {
                                        var resultat = MessageBox.Show("Conseil d'utilisation",
                                            "Pour modifier plus facilement votre calcul, il vous suffit simplement d'appuyer sur celui.",
                                            new[] {"Quitter l'aide", "Continuer"}).Result;
                                        if (resultat.HasValue)
                                        {
                                            if (resultat.Value == 1)
                                            {
                                                var resultat2 = MessageBox.Show("Conseil d'utilisation",
                                                    "Lorsque vous appuyer sur le bouton \"C\" votre calcul est enregistrer dans l'historique.",
                                                    new[] {"Quitter l'aide", "Continuer"}).Result;
                                                if (resultat2.HasValue)
                                                {
                                                    if (resultat2.Value == 1)
                                                    {
                                                        MessageBox.Show("Conseil d'utilisation",
                                                            "Vous pouvez accéder à l'historique depuis le bouton \"Hist\" et le quitter via le bouton \"Retour\" ou via la touche Back/Retour de votre appareil.",
                                                            new[] {"OK"});
                                                    }
                                                }
                                            }
                                        }
                                    }, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously,
                                    TaskScheduler.Current);
                            }

                            string valeurBouton = "";

                            foreach (Tuple<string, Rectangle> t1 in ListeRectangleBouton)
                            {
                                if (t1.Item2.Contains(t.Position.X, t.Position.Y))
                                {
                                    valeurBouton = t1.Item1;
                                }
                            }

                            switch (valeurBouton)
                            {
                                case "C":
                                    bool bonCalcul;

                                    try
                                    {
                                        Expression computeCalculation = new Expression(_calcul);


                                        float.Parse(computeCalculation.Evaluate().ToString());
                                        bonCalcul = true;

                                    }
                                    catch
                                    {
                                        bonCalcul = false;
                                    }

                                    if (bonCalcul)
                                    {
                                        ListeHistoriqueCalcul.Add(new Tuple<string, string>(_calcul, _calculGraphique));
                                    }

                                    _calcul = "";
                                    _calculGraphique = "";
                                    _resultat = 0;
                                    break;
                                case "<-":
                                    if (_calcul?.Length > 0)
                                    {
                                        try
                                        {
                                            _calcul = _calcul.Remove(_calcul.Length - 1);
                                            if (_calculGraphique[_calculGraphique.Length - 1] == '\n')
                                            {
                                                _calculGraphique = _calculGraphique.Remove(_calculGraphique.Length - 2,
                                                    2);
                                            }
                                            else
                                            {
                                                _calculGraphique = _calculGraphique.Remove(_calculGraphique.Length - 1);
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    break;
                                case "Hist":
                                    _affichageHistorique = true;
                                    break;
                                default:

                                    int maxLigne =
                                        (int) (_ecartResultatCalcul / (_policeEcriture.MeasureString("A").Y / 1.35f));

                                    if (_calculGraphique != null && _calcul != null)
                                    {
                                        if (_calculGraphique.Length > 0)
                                        {
                                            int nombreLigne = 0;
                                            foreach (var t1 in _calculGraphique)
                                            {
                                                if (t1 == '\n')
                                                {
                                                    nombreLigne++;
                                                }
                                            }

                                            if (nombreLigne < maxLigne)
                                            {
                                                if (_calcul.Length > 0 && _calculGraphique.Length > 0)
                                                {
                                                    if (valeurBouton == "+" || valeurBouton == "-" ||
                                                        valeurBouton == "/" ||
                                                        valeurBouton == "x" &&
                                                        (_calcul[_calcul.Length - 1] != '+' &&
                                                         _calcul[_calcul.Length - 1] != '-' &&
                                                         _calcul[_calcul.Length - 1] != '/' &&
                                                         _calcul[_calcul.Length - 1] != '*'))
                                                    {
                                                        if (valeurBouton == "x")
                                                        {
                                                            _calcul += "*";
                                                            _calculGraphique += valeurBouton;
                                                        }
                                                        else
                                                        {
                                                            _calcul += valeurBouton;
                                                            _calculGraphique += valeurBouton;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _calcul += valeurBouton;
                                                        _calculGraphique += valeurBouton;
                                                    }

                                                    if (_calculGraphique.Contains("\n"))
                                                    {
                                                        int position = 0;
                                                        for (int i = 0; i < _calculGraphique.Length; i++)
                                                        {
                                                            if (_calculGraphique[i] == '\n')
                                                            {
                                                                position = i;
                                                            }
                                                        }

                                                        if (position < _calculGraphique.Length)
                                                        {
                                                            string tmpCalcu = _calculGraphique.Substring(position);
                                                            var fontSize = _policeEcriture.MeasureString(tmpCalcu);
                                                            if (fontSize.X / 1.35f > GraphicsDevice.Viewport.Width)
                                                            {
                                                                _calculGraphique += "\n";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var fontSize =
                                                                _policeEcriture.MeasureString(_calculGraphique);
                                                            if (fontSize.X / 1.35f > GraphicsDevice.Viewport.Width)
                                                            {
                                                                _calculGraphique += "\n";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var fontSize = _policeEcriture.MeasureString(_calculGraphique);
                                                        if (fontSize.X / 1.35f > GraphicsDevice.Viewport.Width)
                                                        {
                                                            _calculGraphique += "\n";
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Limite atteinte",
                                                    "Limite de la longueur du calcul atteinte",
                                                    new[] {"OK"});
                                            }
                                        }
                                        else
                                        {

                                            if (valeurBouton == "+" || valeurBouton == "-" || valeurBouton == "/" ||
                                                valeurBouton == "x" &&
                                                (_calcul[_calcul.Length - 1] != '+' &&
                                                 _calcul[_calcul.Length - 1] != '-' &&
                                                 _calcul[_calcul.Length - 1] != '/' &&
                                                 _calcul[_calcul.Length - 1] != '*'))
                                            {
                                                if (valeurBouton == "x")
                                                {
                                                    _calcul += "*";
                                                    _calculGraphique += valeurBouton;
                                                }
                                                else
                                                {
                                                    _calcul += valeurBouton;
                                                    _calculGraphique += valeurBouton;
                                                }
                                            }
                                            else
                                            {
                                                _calcul += valeurBouton;
                                                _calculGraphique += valeurBouton;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (valeurBouton == "+" || valeurBouton == "-" || valeurBouton == "/" ||
                                            valeurBouton == "x" &&
                                            (_calcul[_calcul.Length - 1] != '+' && _calcul[_calcul.Length - 1] != '-' &&
                                             _calcul[_calcul.Length - 1] != '/' && _calcul[_calcul.Length - 1] != '*'))
                                        {
                                            if (valeurBouton == "x")
                                            {
                                                _calcul += "*";
                                                _calculGraphique += valeurBouton;
                                            }
                                            else
                                            {
                                                _calcul += valeurBouton;
                                                _calculGraphique += valeurBouton;
                                            }
                                        }
                                        else
                                        {
                                            _calcul += valeurBouton;
                                            _calculGraphique += valeurBouton;
                                        }

                                        if (_calculGraphique.Contains("\n"))
                                        {
                                            int position = 0;
                                            for (int i = 0; i < _calculGraphique.Length; i++)
                                            {
                                                if (_calculGraphique[i] == '\n')
                                                {
                                                    position = i;
                                                }
                                            }

                                            if (position < _calculGraphique.Length)
                                            {
                                                string tmpCalcu = _calculGraphique.Substring(position);
                                                var fontSize = _policeEcriture.MeasureString(tmpCalcu);
                                                if (fontSize.X / 1.35f > GraphicsDevice.Viewport.Width)
                                                {
                                                    _calculGraphique += "\n";
                                                }
                                            }
                                            else
                                            {
                                                var fontSize = _policeEcriture.MeasureString(_calculGraphique);
                                                if (fontSize.X / 1.35f > GraphicsDevice.Viewport.Width)
                                                {
                                                    _calculGraphique += "\n";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var fontSize = _policeEcriture.MeasureString(_calculGraphique);
                                            if (fontSize.X / 1.35f > GraphicsDevice.Viewport.Width)
                                            {
                                                _calculGraphique += "\n";
                                            }
                                        }
                                    }

                                    break;
                            }

                            if (_calcul != null)
                            {
                                float tmpResultat = 0;
                                bool ok;
                                try
                                {
                                    Expression computeCalculation = new Expression(_calcul);

                                    tmpResultat = float.Parse(computeCalculation.Evaluate().ToString());
                                    ok = true;

                                }
                                catch
                                {
                                    ok = false;
                                }

                                if (ok)
                                {
                                    _resultat = tmpResultat;
                                }
                                else
                                {
                                    _resultat = 0;
                                }
                            }
                        }
                        else
                        {
                            if (RectangleBoutonViderHistorique.Contains(t.Position))
                            {
                                ListeHistoriqueCalcul.Clear();
                            }

                            if (RectangleBoutonRetourHistorique.Contains(t.Position))
                            {
                                _affichageHistorique = false;
                                _detectionScroll = false;
                                _defilementCalcul = 0;
                            }

                            if (_detectionScroll == false)
                            {
                                if (_orientationEcran == DisplayOrientation.Portrait ||
                                    _orientationEcran == DisplayOrientation.PortraitDown)
                                {
                                    if (t.Position.Y > (float) (GraphicsDevice.Viewport.Width * 50) / 100)
                                    {
                                        _directionDefilement = 1;
                                        _detectionScroll = true;
                                    }
                                    else
                                    {
                                        _directionDefilement = 0;
                                        _detectionScroll = true;
                                    }
                                }
                            }
                            else
                            {
                                _detectionScroll = false;
                            }

                        }

                        _compteurClique = 0;
                    }
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            CalculPositionGraphique();

            try
            {
                TouchFonction();
            }
            catch (Exception erreur)
            {
#if DEBUG
                Console.WriteLine("Erreur: " + erreur.Message);
#endif
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {

                _affichageHistorique = false;

            }
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            spriteBatch.Begin();

            if (_affichageHistorique == false)
            {
                if (ListeRectangleBouton.Count > 0)
                {
                    foreach (Tuple<string, Rectangle> t in ListeRectangleBouton)
                    {
                        int positionEcritureX = t.Item2.X + ((t.Item2.Width * 10) / 100);
                        int positionEcritureY = t.Item2.Y + ((t.Item2.Height * 10) / 100);
                        if (t.Item1 != "Hist")
                        {
                            spriteBatch.Draw(_boutonNormalTexture, t.Item2, Color.White);
                            spriteBatch.DrawString(_policeEcriture, t.Item1,
                                new Vector2(positionEcritureX, positionEcritureY), Color.LightBlue, 0.0f,
                                Vector2.One,
                                1f, SpriteEffects.None, 0.0f);
                        }

                        else
                        {
                            spriteBatch.Draw(_boutonEgaleTexture, t.Item2, Color.White);
                            if (_policeEcriture.MeasureString("Historique").X < t.Item2.Width)
                            {
                                spriteBatch.DrawString(_policeEcriture, "Historique",
                                    new Vector2(positionEcritureX, positionEcritureY), Color.LightBlue);
                            }
                            else
                            {
                                spriteBatch.DrawString(_policeEcriture, t.Item1,
                                    new Vector2(positionEcritureX, positionEcritureY), Color.LightBlue);
                            }
                        }
                    }

                    if (_calculGraphique != null)
                    {

                        spriteBatch.DrawString(_policeEcriture, _calculGraphique,
                            new Vector2(RectangleCalcul.X, RectangleCalcul.Y),
                            Color.Black, 0.0f, Vector2.One, 0.7f, SpriteEffects.None, 0.0f);
                    }


                    spriteBatch.DrawString(_policeEcriture, FixAffichageResultatExponentiel(_resultat),
                        new Vector2(RectangleResultat.X, RectangleResultat.Y),
                           Color.Black);
                    

                    spriteBatch.Draw(_boutonAideTexture, RectangleBoutonInterrogation, Color.White);
                }
            }
            else // Affichage de l'historique
            {


                float baseY = (float) (GraphicsDevice.Viewport.Height * 15) / 100;

                for (int i = 0; i < ListeHistoriqueCalcul.Count; i++)
                {
                    Expression computeCalculation = new Expression(ListeHistoriqueCalcul[i].Item1);
                    
                        float calcul =
                            float.Parse(computeCalculation.Evaluate().ToString());

                        spriteBatch.DrawString(_policeEcriture,
                            (i + 1) + " - " + ListeHistoriqueCalcul[i].Item2 + " = " + FixAffichageResultatExponentiel(calcul),
                            new Vector2(RectangleCalcul.X, baseY + _defilementCalcul),
                            Color.Black, 0.0f, Vector2.One, 0.35f, SpriteEffects.None, 0.0f);

                        int compteurSaut = 0;
                        if (ListeHistoriqueCalcul[i].Item2.Contains("\n"))
                        {
                            compteurSaut += ListeHistoriqueCalcul[i].Item2.Count(t => t == '\n');
                        }

                        baseY += ((float) (GraphicsDevice.Viewport.Height * 4) / 100) +
                                 (((float) GraphicsDevice.Viewport.Height * (compteurSaut * 2)) / 100);
                    
                }

                spriteBatch.DrawString(_policeEcriture, "Vos historiques de calculs:",
                    new Vector2(RectangleCalcul.X + (((float) (GraphicsDevice.Viewport.Width * 4) / 100)),
                        ((float) (GraphicsDevice.Viewport.Height * 5) / 100)),
                    Color.Black, 0.0f, Vector2.One, 0.7f, SpriteEffects.None, 0.0f);

                spriteBatch.Draw(_boutonNormalTexture, RectangleBoutonRetourHistorique, Color.White);
                spriteBatch.DrawString(_policeEcriture, "Retour",
                    new Vector2(
                        RectangleBoutonRetourHistorique.X + ((RectangleBoutonRetourHistorique.Width * 20) / 100),
                        RectangleBoutonRetourHistorique.Y + ((RectangleBoutonRetourHistorique.Height) * 27 / 100)),
                    Color.DodgerBlue, 0.0f, Vector2.One, 0.55f, SpriteEffects.None, 0.0f);

                spriteBatch.Draw(_boutonEgaleTexture, RectangleBoutonViderHistorique, Color.White);
                spriteBatch.DrawString(_policeEcriture, "Vider vos historiques",
                    new Vector2(RectangleBoutonViderHistorique.X + ((RectangleBoutonViderHistorique.Width * 20) / 100),
                        RectangleBoutonViderHistorique.Y + ((RectangleBoutonViderHistorique.Height) * 27 / 100)),
                    Color.DodgerBlue, 0.0f, Vector2.One, 0.55f, SpriteEffects.None, 0.0f);


            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Corrige l'affichage du résultat lorsque celui-ci est long.
        /// </summary>
        /// <param name="resultat"></param>
        /// <returns></returns>
        private string FixAffichageResultatExponentiel(float resultat)
        {
            string resultatString = string.Format("{0:f99}", resultat).TrimEnd('0');
            if (resultatString.EndsWith(",")) resultatString = resultatString.Remove(resultatString.Length - 1);

            return resultatString;
        }
    }
}
