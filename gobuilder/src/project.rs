#[derive(Clone)]
pub struct Project {
    pub(crate) name: String,
    pub(crate) post_build: Option<fn(&str, &str, bool)>,
    pub(crate) dependencies: Vec<String>
}

impl Project {
    pub(crate) fn new(name: &str, post_build: Option<fn(&str, &str, bool)>, dependencies: Vec<&str>) -> Self {
        Self {
            name: name.to_string(),
            post_build,
            dependencies: dependencies.into_iter().map(|s| s.to_string()).collect()
        }
    }
}