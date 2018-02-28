using System;
using System.Windows.Forms;
using System.Drawing;
using AICheckers;

public class Fen : Form
{
    public Fen()
    {
        //SuspendLayout();
        Text = @"CheckersIA";    // Le titre de la fenêtre
        Size = new Size(300, 300);        // La taille initiale
        MinimumSize = new Size(200, 150); // La taille minimale

        // Le label 
        var message = new Label
        {
            Text = @"Bienvenue sur CheckersIA !",
            AutoSize = true,
            Location = new Point(50, 30)
        };
        // Taille selon le contenu
        // Position x=50 y=30

        // Le bouton "JvJ"
        // Taille selon le contenu
        // Position x=50 y=60
        var jvj = new Button
        {
            Text = @"JvJ",
            AutoSize = true,
            Location = new Point(50, 90)
        };
        jvj.Click += jvj_Click;
        
        // Le bouton "JvIA"
        var jvIa = new Button
        {
            Text = @"JvIA",
            AutoSize = true,
            Location = new Point(50, 120)
        };
        jvIa.Click += jvIa_Click;
        
        // Le bouton "IAvIA"
        var iavIa = new Button
        {
            Text = @"IAvIA",
            AutoSize = true,
            Location = new Point(50, 150)
        };
        iavIa.Click += iavIa_Click;
        
        // Le bouton "Fermer"
        var fermer = new Button
        {
            Text = @"Fermer",
            AutoSize = true,
            Location = new Point(50, 180)
        };
        fermer.Click += fermer_Click;
        
        // Ajouter les composants à la fenêtre 
        Controls.Add(message);
        Controls.Add(jvj);
        Controls.Add(jvIa);
        Controls.Add(iavIa);
        Controls.Add(fermer);

        ResumeLayout(false);
        PerformLayout();
    }

    public sealed override Size MinimumSize
    {
        get { return base.MinimumSize; }
        set { base.MinimumSize = value; }
    }

    public sealed override string Text
    {
        get { return base.Text; }
        set { base.Text = value; }
    }

    // Gestionnaire d'événement
    private void jvj_Click(object sender, EventArgs e)
    {
        FormMain formMain = new FormMain("jvj");
        formMain.Show();
    }

    private void jvIa_Click(object sender, EventArgs e)
    {
        FormMain formMain = new FormMain("jvia");
        formMain.Show();
    }
    
    private void iavIa_Click(object sender, EventArgs e)
    {
        FormMain formMain = new FormMain("iavia");
        formMain.Show();
    }
    
    private void fermer_Click(object sender, EventArgs evt)
    {
        // Fin de l'application :
        Application.Exit();
    }

}