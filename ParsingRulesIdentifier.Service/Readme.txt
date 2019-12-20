ParsingRulesIdentifier.Service subscribes to queue of messages <<QUEUE_NAME>> where it gets the tasks to 
try to identify massmedia website parsing rules. 
Saves results to database, sends a message with task status result

Algorythm of work
1. Recieves a message to process a mass media.
2. Creates a task with website processing
	2.1 Opens it with HtmlAgilityPack or Puppertier, gets Html
	2.2 Tries to find RSS pages of website locating on the same domain (excluding subdomains)
		2.2.1 If there are RSS Pages it gets N links from RSS page, opens them and saves their html
		2.2.2 If there are no RSS pages or can't take links to articles from them, 
		it takes N or all links from main page (or it can work with links to common categories links 
		on page like 'culture', 'politics', 'sport' etc) and saves html
	2.3 Across all article html's service tries to find html containers (div/span/article/td etc) that have common traits like id or class
		and that has only 'text' html tags within. If there are many 'unique' rules for a source - success, otherwise job failed
3. Saves results to DB (or sends result to REST API)
4. Sends message with job state (success/ failed) and/or result (rules)