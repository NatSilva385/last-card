import passport from "passport";
import passportLocal from "passport-local";
import { db } from "../db";
import { UsuarioDb } from "../usuario";
const LocalStrategy = passportLocal.Strategy;
passport.use(
  "login",
  new LocalStrategy(
    { usernameField: "email", passwordField: "password" },
    async (email, password, done) => {
      try {
        let userDB = new UsuarioDb(db);
        const user = await userDB.findOne(email);
        console.log(user);
        if (user == null) {
          return done(null, false, { message: "User not found" });
        }
        const hash = userDB.hasher(password, user.salt!);
        console.log(hash);
        if (hash != user.hash) {
          return done(null, false, { message: "Wrong Password" });
        }
        var message = JSON.stringify(user);
        return done(null, user, { message });
      } catch (error) {
        return done(error);
      }
    }
  )
);

passport.serializeUser(function (user: any, done) {
  done(null, user.id);
});

passport.deserializeUser(function (id: number, done) {
  let userDB = new UsuarioDb(db);
  userDB.findUserById(id).then((user) => {
    done(null, user);
  });
});
