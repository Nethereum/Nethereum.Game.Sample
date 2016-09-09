contract PlayerScore {
    
    uint maxTopScores = 5;

    struct TopScore{
        address addr;
        int score;
    }

    TopScore[] public topScores;

    mapping (address=>int) public userTopScores;

    function setTopScore(int score) {
        var currentTopScore = userTopScores[msg.sender];
        if(currentTopScore < score){
            userTopScores[msg.sender] = score;
        }

        if(topScores.length < maxTopScores){
            var topScore = TopScore(msg.sender, score);
            topScores.push(topScore);
        }else{
            int lowestScore = 0;
            uint lowestScoreIndex = 0; 
            for (uint i = 0; i < topScores.length; i++)
            {
                TopScore currentScore = topScores[i];
                if(i == 0){
                    lowestScore = currentScore.score;
                    lowestScoreIndex = i;
                }else{
                    if(lowestScore > currentScore.score){
                        lowestScore = currentScore.score;
                        lowestScoreIndex = i;
                    }
                }
            }
            if(score > lowestScore){
                var newtopScore = TopScore(msg.sender, score);
                topScores[lowestScoreIndex] = newtopScore;
            }
        }
    }

    function getCountTopScores() returns(uint) {
        return topScores.length;
    }
}