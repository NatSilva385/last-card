import passportLocal from "passport-local";
import bcrypt from "bcrypt";
import { db } from "../db";
import { Usuario, UsuarioDb } from "../usuario";

const LocalStrategy = passportLocal.Strategy;
module.exports = function (passport: any) {
  passport.use(
    new LocalStrategy({ usernameField: "email" }, (email, password, done) => {
      console.log("aqui");
      let userDB = new UsuarioDb(db);
      userDB.findOne(email).then((user) => {
        if (user == null) {
          return done(null, false, { message: "that email is not registered" });
        }

        var senha = userDB.hasher(password, user.salt!);

        if (senha == password) {
          return done(null, user);
        } else {
          return done(null, false, { message: "pass incorrect" });
        }
      });
    })
  );

  passport.serializeUser(function (user: any, done: any) {
    console.log("serializing");
    done(null, user.id);
  });

  passport.deserializeUser((id: number, done: any) => {
    let userDB = new UsuarioDb(db);
    userDB.findUserById(id).then((user) => {
      if (user != null) {
        done(null, user!);
      }
    });
  });
};
