INSERT INTO public.exchange (name, restendpoint, websocketendpoint) VALUES ('coinbasepro', 'https://api.pro.coinbase.com', 'wss://ws-feed.pro.coinbase.com');
INSERT INTO public.exchange (name, restendpoint, pollintervalms) VALUES ('kraken', 'https://api.kraken.com/0', 30000);
INSERT INTO public.exchange (name, restendpoint, reversedcurrencypairs) VALUES ('bittrex', 'https://bittrex.com', true);
INSERT INTO public.exchange (name, restendpoint) VALUES ('binance', 'https://api.binance.com');
INSERT INTO public.exchange (name, restendpoint) VALUES ('bitstamp', 'https://www.bitstamp.net');

INSERT INTO public.currency (symbol) VALUES('BTC'); 
INSERT INTO public.currency (symbol) VALUES('DOGE'); 
INSERT INTO public.currency (symbol) VALUES('BCH'); 

--ALIASES

--KRAKEN

WITH  
exchangeid as (SELECT id FROM exchange WHERE name='kraken'), 
currencyid as (SELECT id FROM currency WHERE symbol='BTC') 
INSERT INTO public.currencyalias (alias, exchangeid, currencyid)  
SELECT 'XBT', exchangeid.id, currencyid.id FROM  
exchangeid,currencyid;

WITH  
exchangeid as (SELECT id FROM exchange WHERE name='kraken'), 
currencyid as (SELECT id FROM currency WHERE symbol='DOGE') 
INSERT INTO public.currencyalias (alias, exchangeid, currencyid)  
SELECT 'XDG', exchangeid.id, currencyid.id FROM  
exchangeid,currencyid;

WITH  
exchangeid as (SELECT id FROM exchange WHERE name='binance'), 
currencyid as (SELECT id FROM currency WHERE symbol='BCH') 
INSERT INTO public.currencyalias (alias, exchangeid, currencyid)  
SELECT 'BCC', exchangeid.id, currencyid.id FROM  
exchangeid,currencyid;


