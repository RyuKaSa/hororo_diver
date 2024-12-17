# hororo_diver
## Config scene notes
### Peindre sur la Tilemap avec la Palette

Pour commencer à peindre sur la **Tilemap** à l'aide de la palette, suivez les étapes ci-dessous :

1. **Ouvrir la Tile Palette**  
   Allez dans le menu **Window > 2D > Tile Palette** pour afficher la fenêtre **Tile Palette**. Assurez-vous d’avoir sélectionné votre palette, comme `test_palette`.

2. **Peindre sur la Grille (Tilemap)**  
   - Sélectionnez votre `Tilemap` dans la hiérarchie de la scène pour indiquer où vous souhaitez peindre.
   - Utilisez l’outil **Brush** (pinceau) de la **Tile Palette** pour peindre les tiles sur la grille.

3. **Problèmes d’affichage des Sprites ?**  
   Si les sprites n'apparaissent pas lorsque vous peignez, vérifiez les paramètres suivants :
   - Dans les **Import Settings** de l'image que vous avez importée, assurez-vous que **Pixel Per Unit** est défini sur `1` (dans l’inspecteur).  
   Cela garantit que chaque pixel de votre asset correspondra à une cellule de la grille.


### Ajout NavMesh sur une tilemap avec NavMesh Plus

   #### Création NavMesh Navigation
   - Créer un empty object à la racine utilisant le préfixe NavMesh et ajouter lui la composante "Navigation Surface" (**Attention ce n'est pas la composant NavMesh Surface**)
   - Ajouter ensuite la composante "Navigation CollectSources2d puis cliquer sur Rotate Surface to XY
   - Cliquer ensuite sur Bake  (**Attention** Veiller à ce que le z de votre grid soit égale à celui de votre Navigation Surface)
   
   #### Ajout Obstacle NavMesh
   - Pour ajouter des obstacles, ajoutez une tilemap enfant à grid et dessiner le/les obstacles à l'aide de la palette
   - Ajoutez la composante "Navigation Modifier", cocher "Override Area" puis dans "Area" selectionner "Not walkable"
   - Rebake le Navigation Surface et normalement c'est bon

   #### Ajout Agent
   - Dans la composante qui va être un agent, ajouter la composante "Nav Mesh Agent" puis la composante "Agent Override 2d (Script)"

   #### Tester l'agent
   Dans le répertoire Debug, il y a le script AgentController permettant de set la destination de l'agent via un clic gauche