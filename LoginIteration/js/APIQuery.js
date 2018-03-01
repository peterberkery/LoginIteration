var header = document.querySelector('header');
var section = document.querySelector('section');

// call the variables from the APi URL
// requestURL variable stored for later use
var requestURL = 'https://api.coinmarketcap.com/v1/ticker/?convert=EUR&limit=5';
var request = new XMLHttpRequest();
// HTTP method 'GET' to simply retrieve the coin data needed
// 'requestURL' the URL to make the request to, which was stored above
request.open('GET', requestURL);
// set responseType to JSON so that xmlHttpRequest knows the server will be returning JSON & will need to be converted behind the scenes to a JS object
// send request using 'send' method'
request.responseType = 'json';
request.send();
// this section waits for response to return for server
// wrapping code in an event handler when the load event fires on the request obect 'onload'
//this is because the load event fires when the reponse has successfully returned
//this guarantees that 'request.response' will defo be available when we want to do something with it
request.onload = function () {
    // storing the server reponse to our request in 'response' property in variable calles 'prices'
    // this variable will now contain the JS object based on the JSON response
    var response = request.response;
    // passing the object to two function calls:
    // the first will fill <header> with the appropriate data
    populateHeader(response);
    // will create an information card for each price we wish to examine
    showPrices(response);
}
//Populate Header function
function populateHeader(jsonObj) {
    var myH1 = document.createElement('h1');
    myH1.textContent = jsonObj['squadName'];
    header.appendChild(myH1);

    var myPara = document.createElement('p');
    myPara.textContent = 'Hometown: ' + jsonObj['homeTown'] + ' // Formed: ' + jsonObj['formed'];
    header.appendChild(myPara);
}
//Populate Section function with all prices
// call the jsonObj which was stored previously
function showPrices(jsonObj) {
    var prices = jsonObj;
    // 
    for (var i = 0; i < prices.length; i++) {
        var myArticle = document.createElement('article');
        var myH2 = document.createElement('h2');
        var myPara1 = document.createElement('p');
        var myPara2 = document.createElement('p');
        var myPara3 = document.createElement('p');
        var myPara4 = document.createElement('p');
        var myPara5 = document.createElement('p');
        var myList = document.createElement('ul');

        myH2.textContent = prices[i].id;
        myPara1.textContent = 'Price in US Dollar: ' + prices[i].price_usd;
        myPara2.textContent = 'Price in Euro: ' + prices[i].price_eur;
        myPara3.textContent = '% Change 1 Hour' + prices[i].percent_change_1h;
        myPara4.textContent = '% Change 24 Hours: ' + prices[i].percent_change_24h;
        myPara5.textContent = '% Change 7 Days: ' + prices[i].percent_change_7d;


        //var superPowers = heroes[i].powers;
        //for (var j = 0; j < superPowers.length; j++) {
        //    var listItem = document.createElement('li');
        //    listItem.textContent = superPowers[j];
        //    myList.appendChild(listItem);
        //}

        myArticle.appendChild(myH2);
        myArticle.appendChild(myPara1);
        myArticle.appendChild(myPara2);
        myArticle.appendChild(myPara3);
        myArticle.appendChild(myPara4);
        myArticle.appendChild(myPara5);
        //myArticle.appendChild(myList);
        section.appendChild(myArticle);
    }
}