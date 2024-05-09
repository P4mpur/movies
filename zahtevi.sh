#/bin/bash -x -v
movies=(
	"The\ Shawshank\ Redemption" "The Godfather" "The Godfather: Part II" "The Dark Knight" "12 Angry Men" "Schindler's List" "The Lord of the Rings: The Return of the King" "Pulp Fiction" "The Good, the Bad and the Ugly" "The Lord of the Rings: The Fellowship of the Ring" "Fight Club" "Forrest Gump" "Inception" "The Lord of the Rings: The Two Towers" "Star Wars: Episode V - The Empire Strikes Back" "The Matrix" "Goodfellas" "One Flew Over the Cuckoo's Nest" "Seven Samurai" "Se7en" "Life Is Beautiful" "City of God" "The Silence of the Lambs" "It's a Wonderful Life" "Star Wars: Episode IV - A New Hope" "Saving Private Ryan" "Spirited Away" "The Green Mile" "Parasite" "Interstellar" "Léon: The Professional" "The Usual Suspects" "Harakiri" "The Lion King" "Back to the Future" "The Pianist" "Terminator 2: Judgment Day" "American History X" "Modern Times" "Psycho" "Gladiator" "City Lights" "The Departed" "The Intouchables" "Whiplash" "The Prestige" "Casablanca" "Cinema Paradiso" "Rear Window" "Alien")

for movie in "${movies[@]}"; do
	
	#Bez zapisivanja u fajl
	#curl http://localhost:8083/$movie
	

	#sa zapisivanjem u fajlu
	curl http://localhost:8083/$movie > "filmovi/$movie.txt"
    	
done
