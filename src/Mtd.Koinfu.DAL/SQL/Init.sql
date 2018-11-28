/*
NAMING CONVENTIONS:
- all singular names except where it is unavoidable 
- no separator between words (pssql renders everything lowercase so we cannot use PascalCase or camelCase)
- use underscore to create tables that express many to many relations between tables

*/

-- Database: koinfu

DROP DATABASE koinfu;

CREATE DATABASE koinfu
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;



/* MIND THE ORDER!! it must follow the dependecy order or it will throw and error, same with creation scripts */

DROP TABLE public.order;
DROP TABLE public.tick;
DROP TABLE public.fiatexchangerate;
DROP TABLE public.exchange_currencypair;
DROP TABLE public.currencypair;
DROP TABLE public.currencyalias;
DROP TABLE public.currency;
DROP TABLE public.exchange;


CREATE TABLE public.exchange (
    id serial NOT NULL,
    "name" varchar(50) NULL,
    websocketendpoint varchar(100) NULL,
    restendpoint varchar(100) NULL,
    pollintervalms int4 NULL,
    reversedcurrencypairs boolean NULL,
    CONSTRAINT exchange_name_key UNIQUE (name),
    CONSTRAINT exchange_pkey PRIMARY KEY (id),
    CONSTRAINT exchange_restendpoint_key UNIQUE (restendpoint),
    CONSTRAINT exchange_websocketendpoint_key UNIQUE (websocketendpoint)
)
WITH (
    OIDS=FALSE
) ;


CREATE TABLE public.currency (
    id serial NOT NULL,
    symbol varchar(10) NOT NULL,
    CONSTRAINT currency_pkey PRIMARY KEY (id),
    CONSTRAINT currency_un UNIQUE (symbol)
)
WITH (
    OIDS=FALSE
) ;

CREATE TABLE public.currencyalias (
    id serial NOT NULL,
    alias varchar(10) NOT NULL,
    currencyid serial NOT NULL,
    exchangeid serial NOT NULL,
    CONSTRAINT currencyalias_pkey PRIMARY KEY (id),
    CONSTRAINT currencyalias_un UNIQUE ( currencyid, exchangeid), -- IMPORTANT only 1 alias allowed for currency for every exchange. If there would be more aliases, in the case i have to post data to the exchange which alias should i pick? There would be the need to add a "default" flag or something. In that case i would be able to receive data from multiple currency aliases but i would be able to post data using a predefined one.
    CONSTRAINT currencyalias_exchangeid_fkey FOREIGN KEY (exchangeid) REFERENCES exchange(id),
    CONSTRAINT currencyalias_currencyid_fkey FOREIGN KEY (currencyid) REFERENCES currency(id)
)
WITH (
    OIDS=FALSE
) ;



CREATE TABLE public.currencypair (
    id serial NOT NULL,
    basecurrencyid serial NOT NULL,
    countercurrencyid serial NOT NULL,
    "timestamp" timestamp NULL,
    CONSTRAINT currencypair_pkey PRIMARY KEY (id),
    CONSTRAINT currencypair_un UNIQUE (basecurrencyid, countercurrencyid),
    CONSTRAINT currencypair_basecurrencyid_fkey FOREIGN KEY (basecurrencyid) REFERENCES currency(id),
    CONSTRAINT currencypair_countercurrencyid_fkey FOREIGN KEY (countercurrencyid) REFERENCES currency(id)

)
WITH (
    OIDS=FALSE
) ;


CREATE TABLE public.exchange_currencypair (
    exchangeid serial NOT NULL,
    currencypairid serial NOT NULL,
    CONSTRAINT exchange_currencypair_currencypairid_fkey FOREIGN KEY (currencypairid) REFERENCES currencypair(id),
    CONSTRAINT exchange_currencypair_exchangeid_fkey FOREIGN KEY (exchangeid) REFERENCES exchange(id)
)
WITH (
    OIDS=FALSE
) ;


CREATE TABLE public.fiatexchangerate (
    id serial NOT NULL,
    currencypair varchar(10) NULL,
    rate numeric NULL,
    "timestamp" timestamp NULL,
    CONSTRAINT fiatexchangerate_pkey PRIMARY KEY (id)

)
WITH (
    OIDS=FALSE
) ;


CREATE TABLE public."order" (
    id serial NOT NULL,
    exchangeid serial NOT NULL,
    currencypairid serial NOT NULL,
    externalguid uuid NULL,
    price numeric NULL,
    "size" numeric NULL,
    side varchar(10) NULL,
    status varchar(10) NULL,
    "type" varchar(10) NULL,
    "timestamp" timestamp NULL,
    CONSTRAINT order_pkey PRIMARY KEY (id),
    CONSTRAINT order_currencypairid_fkey FOREIGN KEY (currencypairid) REFERENCES currencypair(id),
    CONSTRAINT order_exchangeid_fkey FOREIGN KEY (exchangeid) REFERENCES exchange(id)
)
WITH (
    OIDS=FALSE
) ;


CREATE TABLE public.tick (
    id serial NOT NULL,
    exchangeid serial NOT NULL,
    currencypairid serial NOT NULL,
    bidprice numeric NULL,
    askprice numeric NULL,
    "timestamp" timestamp NULL,
    CONSTRAINT tick_pkey PRIMARY KEY (id),
    CONSTRAINT tick_currencypairid_fkey FOREIGN KEY (currencypairid) REFERENCES currencypair(id),
    CONSTRAINT tick_exchangeid_fkey FOREIGN KEY (exchangeid) REFERENCES exchange(id)
)
WITH (
    OIDS=FALSE
) ;



