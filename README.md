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

