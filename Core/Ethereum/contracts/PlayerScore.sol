contract PlayerScore {
    mapping (address=>int) public topScore;

    function setRecord(int topScore) {
        if(topScore[msg.sender] < topScore){
            topScore[msg.sender] = topScore;
        }
    }
}