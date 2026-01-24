use dotenvy::dotenv;
use openssl::ssl::{SslConnector, SslMethod};
use postgres::Pool;
use postgres_openssl::MakeTlsConnector;
use std::env;

async fn database() -> PgPool {
    dotenv()?;
    let conn_string = env::var("DATABASE_URL")?;
    let pool = sqlx::postgres::PgPool::connect(conn_string).await.unwrap();

    pool
    // let conn_string = env::var("DATABASE_URL")?;
    // let builder = SslConnector::builder(SslMethod::tls())?;
    // let connector = MakeTlsConnector::new(builder.build());
    // let mut pool = Pool::connect(&conn_string, connector)?;
    // println!("Connection established");
    // Ok(())
}
